using Financa.Domain.Entities;

namespace Financa.Domain.Interfaces;

public interface IPasswordResetRepository
{
    Task AddAsync(PasswordResetCode code, CancellationToken ct = default);
    Task<PasswordResetCode?> GetValidCodeAsync(string email, string code, CancellationToken ct = default);
    Task InvalidatePreviousCodesAsync(int userId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
