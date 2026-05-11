using Financa.Data.Context;
using Financa.Domain.Entities;
using Financa.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Financa.Data.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(int id, int userId, CancellationToken ct = default) =>
        await context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

    public async Task<List<Transaction>> GetRecentAsync(int userId, int take, CancellationToken ct = default) =>
        await context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Take(take)
            .ToListAsync(ct);

    public async Task<List<Transaction>> GetByMonthAsync(int userId, int month, int year, CancellationToken ct = default) =>
        await context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.Date.Month == month && t.Date.Year == year)
            .OrderByDescending(t => t.Date)
            .ToListAsync(ct);

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default) =>
        await context.Transactions.AddAsync(transaction, ct);

    public Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        context.Transactions.Update(transaction);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Transaction transaction, CancellationToken ct = default)
    {
        context.Transactions.Remove(transaction);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetByUserAsync(int userId, string? type = null, CancellationToken ct = default) =>
        await context.Categories
            .Where(c => c.UserId == userId && c.IsActive && (type == null || c.Type == type))
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(int id, int userId, CancellationToken ct = default) =>
        await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive, ct);

    public async Task AddAsync(Category category, CancellationToken ct = default) =>
        await context.Categories.AddAsync(category, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}

public class MonthlyGoalRepository(AppDbContext context) : IMonthlyGoalRepository
{
    public async Task<MonthlyGoal?> GetByMonthAsync(int userId, int month, int year, CancellationToken ct = default) =>
        await context.MonthlyGoals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.Month == month && g.Year == year, ct);

    public async Task AddAsync(MonthlyGoal goal, CancellationToken ct = default) =>
        await context.MonthlyGoals.AddAsync(goal, ct);

    public Task UpdateAsync(MonthlyGoal goal, CancellationToken ct = default)
    {
        context.MonthlyGoals.Update(goal);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
