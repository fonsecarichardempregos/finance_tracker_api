using Financa.Contracts.Auth;
using Financa.Domain.Common;

namespace Financa.Application.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>>          LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<RegisterResponse>>       RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result<ChangePasswordResponse>> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);
}
