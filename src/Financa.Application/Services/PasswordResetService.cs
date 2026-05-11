using System.Security.Cryptography;
using System.Text;
using Financa.Application.Interfaces;
using Financa.Contracts.Auth;
using Financa.Domain.Common;
using Financa.Domain.Entities;
using Financa.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Financa.Application.Services;

public class PasswordResetService(
    IUserRepository userRepository,
    IPasswordResetRepository resetRepository,
    ILogger<PasswordResetService> logger) : IPasswordResetService
{
    public async Task<Result<ForgotPasswordResponse>> RequestCodeAsync(
        ForgotPasswordRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("Password reset requested for: {Email}", request.Email);

        var user = await userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null || !user.IsActive)
            return Result<ForgotPasswordResponse>.Success(
                new ForgotPasswordResponse("Se este e-mail estiver cadastrado, você receberá o código em breve."));

        await resetRepository.InvalidatePreviousCodesAsync(user.Id, ct);

        var code = GenerateSixDigitCode();
        var resetCode = PasswordResetCode.Create(user.Id, code, expiresInMinutes: 15);

        await resetRepository.AddAsync(resetCode, ct);
        await resetRepository.SaveChangesAsync(ct);

        logger.LogInformation("Reset code for user {UserId}: {Code}", user.Id, code);

        return Result<ForgotPasswordResponse>.Success(
            new ForgotPasswordResponse("Se este e-mail estiver cadastrado, você receberá o código em breve."));
    }

    public async Task<Result<VerifyResetCodeResponse>> VerifyCodeAsync(
        VerifyResetCodeRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("Verifying reset code for: {Email}", request.Email);

        var resetCode = await resetRepository.GetValidCodeAsync(request.Email, request.Code, ct);

        if (resetCode is null)
            return Result<VerifyResetCodeResponse>.Failure(
                "Código inválido ou expirado.",
                ErrorCodes.InvalidResetCode);

        var resetToken = GenerateResetToken(request.Email, resetCode.Id);

        logger.LogInformation("Reset code verified for: {Email}", request.Email);

        return Result<VerifyResetCodeResponse>.Success(
            new VerifyResetCodeResponse(
                Message: "Código verificado com sucesso.",
                ResetToken: resetToken));
    }

    public async Task<Result<ResetPasswordResponse>> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken ct = default)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
            return Result<ResetPasswordResponse>.Failure(
                "A nova senha e a confirmação não coincidem.",
                ErrorCodes.PasswordMismatch);

        var (email, resetCodeId) = ParseResetToken(request.ResetToken);
        if (email is null || resetCodeId is null)
            return Result<ResetPasswordResponse>.Failure(
                "Token inválido ou expirado. Reinicie o processo.",
                ErrorCodes.InvalidResetCode);

        var user = await userRepository.GetByEmailAsync(email, ct);
        if (user is null)
            return Result<ResetPasswordResponse>.Failure(
                "Usuário não encontrado.",
                ErrorCodes.UserNotFound);

        var resetCode = await resetRepository.GetValidCodeAsync(email, string.Empty, ct);
        if (resetCode is null || resetCode.Id != resetCodeId)
            return Result<ResetPasswordResponse>.Failure(
                "Token expirado. Reinicie o processo.",
                ErrorCodes.ExpiredResetCode);

        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.UpdatePassword(newHash);
        resetCode.MarkAsUsed();

        await userRepository.UpdateAsync(user, ct);
        await resetRepository.SaveChangesAsync(ct);

        logger.LogInformation("Password reset successful for user: {UserId}", user.Id);

        return Result<ResetPasswordResponse>.Success(
            new ResetPasswordResponse("Senha redefinida com sucesso!"));
    }

    private static string GenerateSixDigitCode()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        var number = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1_000_000;
        return number.ToString("D6");
    }

    private static string GenerateResetToken(string email, int resetCodeId)
    {
        var payload = $"{email}:{resetCodeId}:{DateTime.UtcNow.Ticks}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }

    private static (string? email, int? resetCodeId) ParseResetToken(string token)
    {
        try
        {
            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts   = payload.Split(':');
            if (parts.Length < 2) return (null, null);
            return (parts[0], int.Parse(parts[1]));
        }
        catch
        {
            return (null, null);
        }
    }
}
