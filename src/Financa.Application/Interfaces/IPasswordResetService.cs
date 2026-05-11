using Financa.Contracts.Auth;
using Financa.Domain.Common;

namespace Financa.Application.Interfaces;

public interface IPasswordResetService
{
    Task<Result<ForgotPasswordResponse>>  RequestCodeAsync(ForgotPasswordRequest request, CancellationToken ct = default);
    Task<Result<VerifyResetCodeResponse>> VerifyCodeAsync(VerifyResetCodeRequest request, CancellationToken ct = default);
    Task<Result<ResetPasswordResponse>>   ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
}
