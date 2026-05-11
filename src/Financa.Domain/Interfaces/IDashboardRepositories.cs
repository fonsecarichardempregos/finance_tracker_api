using Financa.Domain.Entities;

namespace Financa.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(int id, int userId, CancellationToken ct = default);
    Task<List<Transaction>> GetRecentAsync(int userId, int take, CancellationToken ct = default);
    Task<List<Transaction>> GetByMonthAsync(int userId, int month, int year, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Transaction transaction, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface ICategoryRepository
{
    Task<List<Category>> GetByUserAsync(int userId, string? type = null, CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, int userId, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IMonthlyGoalRepository
{
    Task<MonthlyGoal?> GetByMonthAsync(int userId, int month, int year, CancellationToken ct = default);
    Task AddAsync(MonthlyGoal goal, CancellationToken ct = default);
    Task UpdateAsync(MonthlyGoal goal, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
