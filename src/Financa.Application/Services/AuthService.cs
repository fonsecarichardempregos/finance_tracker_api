using System.Security.Claims;
using Financa.Application.Interfaces;
using Financa.Contracts.Auth;
using Financa.Domain.Common;
using Financa.Domain.Entities;
using Financa.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Financa.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    ILogger<AuthService> logger) : IAuthService
{
    // ─────────────────────────────────────────
    //  LOGIN
    // ─────────────────────────────────────────
    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var user = await userRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
        {
            logger.LogWarning("Login failed - user not found: {Email}", request.Email);
            return Result<LoginResponse>.Failure(
                "E-mail ou senha inválidos.",
                ErrorCodes.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Login failed - inactive account: {Email}", request.Email);
            return Result<LoginResponse>.Failure(
                "Conta desativada. Entre em contato com o suporte.",
                ErrorCodes.UserInactive);
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Login failed - invalid password: {Email}", request.Email);
            return Result<LoginResponse>.Failure(
                "E-mail ou senha inválidos.",
                ErrorCodes.InvalidCredentials);
        }

        var token = tokenService.GenerateToken(user);
        logger.LogInformation("Login successful for user: {UserId}", user.Id);

        return Result<LoginResponse>.Success(new LoginResponse(
            AccessToken: token,
            TokenType: "Bearer",
            ExpiresIn: tokenService.ExpiresInSeconds,
            User: ToDto(user)
        ));
    }

    // ─────────────────────────────────────────
    //  CADASTRO
    // ─────────────────────────────────────────
    public async Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("Register attempt for email: {Email}", request.Email);

        // 1. Verifica se e-mail já está em uso
        var emailExists = await userRepository.ExistsByEmailAsync(request.Email, ct);
        if (emailExists)
        {
            logger.LogWarning("Register failed - email already exists: {Email}", request.Email);
            return Result<RegisterResponse>.Failure(
                "Este e-mail já está cadastrado.",
                ErrorCodes.EmailAlreadyExists);
        }

        // 2. Gera hash da senha
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        // 3. Cria o usuário
        var user = User.Create(
            fullName:     request.FullName,
            email:        request.Email,
            passwordHash: passwordHash,
            phone:        request.Phone,
            birthDate:    request.BirthDate
        );

        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        logger.LogInformation("User registered successfully: {UserId}", user.Id);

        return Result<RegisterResponse>.Success(new RegisterResponse(
            Message: "Conta criada com sucesso!",
            User: ToDto(user)
        ));
    }

    // ─────────────────────────────────────────
    //  TROCA DE SENHA (autenticado)
    // ─────────────────────────────────────────
    public async Task<Result<ChangePasswordResponse>> ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("Change password attempt for user: {UserId}", userId);

        // 1. Valida se as novas senhas coincidem
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return Result<ChangePasswordResponse>.Failure(
                "A nova senha e a confirmação não coincidem.",
                ErrorCodes.PasswordMismatch);
        }

        // 2. Busca o usuário
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return Result<ChangePasswordResponse>.Failure(
                "Usuário não encontrado.",
                ErrorCodes.UserNotFound);
        }

        // 3. Valida a senha atual
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            logger.LogWarning("Change password failed - invalid current password: {UserId}", userId);
            return Result<ChangePasswordResponse>.Failure(
                "Senha atual incorreta.",
                ErrorCodes.InvalidCurrentPass);
        }

        // 4. Aplica o novo hash
        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.UpdatePassword(newHash);

        await userRepository.UpdateAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        logger.LogInformation("Password changed successfully for user: {UserId}", userId);

        return Result<ChangePasswordResponse>.Success(
            new ChangePasswordResponse("Senha alterada com sucesso!"));
    }

    // ── Helper ────────────────────────────────
    private static UserDto ToDto(User user) => new(
        Id:        user.Id,
        FullName:  user.FullName,
        Email:     user.Email,
        Phone:     user.Phone,
        BirthDate: user.BirthDate,
        CreatedAt: user.CreatedAt
    );
}
