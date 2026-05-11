using Financa.Data.Context;
using Financa.Domain.Entities;
using Financa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Financa.Data.Repositories;

public class PasswordResetRepository(AppDbContext context) : IPasswordResetRepository
{
    public async Task AddAsync(PasswordResetCode code, CancellationToken ct = default) =>
        await context.PasswordResetCodes.AddAsync(code, ct);

    public async Task<PasswordResetCode?> GetValidCodeAsync(
        string email,
        string code,
        CancellationToken ct = default) =>
        await context.PasswordResetCodes
            .Include(p => p.User)
            .Where(p =>
                p.User!.Email == email.ToLowerInvariant() &&
                (code == string.Empty || p.Code == code) &&
                !p.IsUsed &&
                p.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task InvalidatePreviousCodesAsync(int userId, CancellationToken ct = default)
    {
        var activeCodes = await context.PasswordResetCodes
            .Where(p => p.UserId == userId && !p.IsUsed)
            .ToListAsync(ct);

        foreach (var c in activeCodes)
            c.MarkAsUsed();
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
