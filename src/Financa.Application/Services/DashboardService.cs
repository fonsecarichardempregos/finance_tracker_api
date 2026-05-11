using Financa.Application.Interfaces;
using Financa.Contracts.Dashboard;
using Financa.Domain.Common;
using Financa.Domain.Entities;
using Financa.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Financa.Application.Services;

public class DashboardService(
    ITransactionRepository transactionRepository,
    ICategoryRepository    categoryRepository,
    IMonthlyGoalRepository goalRepository,
    ILogger<DashboardService> logger) : IDashboardService
{
    private static readonly string[] MonthNames =
    [
        "Janeiro","Fevereiro","Março","Abril","Maio","Junho",
        "Julho","Agosto","Setembro","Outubro","Novembro","Dezembro"
    ];

    public async Task<Result<DashboardResponse>> GetDashboardAsync(
        int userId, int month, int year, CancellationToken ct = default)
    {
        var transactions = await transactionRepository.GetByMonthAsync(userId, month, year, ct);
        var recent       = await transactionRepository.GetRecentAsync(userId, 10, ct);
        var goal         = await goalRepository.GetByMonthAsync(userId, month, year, ct);

        var monthIncome  = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
        var monthExpense = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
        var balance      = monthIncome - monthExpense;

        double? goalPercent = goal is not null && goal.TargetAmount > 0
            ? Math.Min(100, Math.Round((double)(monthExpense / goal.TargetAmount) * 100, 1))
            : null;

        var expensesByCategory = transactions
            .Where(t => t.Type == "expense" && t.Category is not null)
            .GroupBy(t => t.Category!)
            .Select(g => new
            {
                Category = g.Key,
                Amount   = g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Amount)
            .Select(x => new CategoryExpenseDto(
                CategoryId: x.Category.Id,
                Name:       x.Category.Name,
                Icon:       x.Category.Icon,
                Color:      x.Category.Color,
                Amount:     x.Amount,
                Percent:    monthExpense > 0
                    ? Math.Round((double)(x.Amount / monthExpense) * 100, 1)
                    : 0))
            .ToList();

        var recentTransactions = recent
            .Where(t => t.Category is not null)
            .Select(t => new RecentTransactionDto(
                Id:       t.Id,
                Name:     t.Category!.Name,
                Category: t.Description ?? t.Category.Name,
                Icon:     t.Category.Icon,
                Color:    t.Category.Color,
                Amount:   t.Type == "income" ? t.Amount : -t.Amount,
                Type:     t.Type,
                Date:     FormatDate(t.Date)))
            .ToList();

        var response = new DashboardResponse(
            Month:               $"{MonthNames[month - 1]} {year}",
            Balance:             balance,
            MonthIncome:         monthIncome,
            MonthExpense:        monthExpense,
            GoalTarget:          goal?.TargetAmount,
            GoalProgressPercent: goalPercent,
            ExpensesByCategory:  expensesByCategory,
            RecentTransactions:  recentTransactions);

        return Result<DashboardResponse>.Success(response);
    }

    public async Task<Result<TransactionResponse>> CreateTransactionAsync(
        int userId, CreateTransactionRequest request, CancellationToken ct = default)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, userId, ct);
        if (category is null)
            return Result<TransactionResponse>.Failure(
                "Categoria não encontrada.", ErrorCodes.ValidationError);

        var transaction = Transaction.Create(
            userId:      userId,
            categoryId:  request.CategoryId,
            amount:      request.Amount,
            type:        request.Type,
            date:        request.Date,
            description: request.Description);

        await transactionRepository.AddAsync(transaction, ct);
        await transactionRepository.SaveChangesAsync(ct);

        logger.LogInformation("Transaction created: {Id} for user {UserId}", transaction.Id, userId);

        return Result<TransactionResponse>.Success(ToTransactionResponse(transaction, category));
    }

    public async Task<Result<TransactionResponse>> DeleteTransactionAsync(
        int userId, int transactionId, CancellationToken ct = default)
    {
        var transaction = await transactionRepository.GetByIdAsync(transactionId, userId, ct);
        if (transaction is null)
            return Result<TransactionResponse>.Failure(
                "Transação não encontrada.", ErrorCodes.UserNotFound);

        await transactionRepository.DeleteAsync(transaction, ct);
        await transactionRepository.SaveChangesAsync(ct);

        return Result<TransactionResponse>.Success(
            ToTransactionResponse(transaction, transaction.Category!));
    }

    public async Task<Result<GoalResponse>> UpsertGoalAsync(
        int userId, UpsertGoalRequest request, CancellationToken ct = default)
    {
        var existing = await goalRepository.GetByMonthAsync(userId, request.Month, request.Year, ct);

        if (existing is not null)
        {
            existing.UpdateTarget(request.TargetAmount);
            await goalRepository.UpdateAsync(existing, ct);
            await goalRepository.SaveChangesAsync(ct);
            return Result<GoalResponse>.Success(ToGoalResponse(existing));
        }

        var goal = MonthlyGoal.Create(userId, request.TargetAmount, request.Month, request.Year);
        await goalRepository.AddAsync(goal, ct);
        await goalRepository.SaveChangesAsync(ct);

        return Result<GoalResponse>.Success(ToGoalResponse(goal));
    }

    private static TransactionResponse ToTransactionResponse(Transaction t, Category c) =>
        new(t.Id, t.CategoryId, c.Name, c.Icon, c.Color, t.Amount, t.Type, t.Description, t.Date, t.CreatedAt);

    private static GoalResponse ToGoalResponse(MonthlyGoal g) =>
        new(g.Id, g.TargetAmount, g.Month, g.Year);

    private static string FormatDate(DateOnly date)
    {
        var today    = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        if (date == today)     return "Hoje";
        if (date == yesterday) return "Ontem";
        return date.ToString("dd MMM");
    }
}
