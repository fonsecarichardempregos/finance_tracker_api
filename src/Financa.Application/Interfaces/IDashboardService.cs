using Financa.Contracts.Dashboard;
using Financa.Domain.Common;

namespace Financa.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardResponse>>      GetDashboardAsync(int userId, int month, int year, CancellationToken ct = default);
    Task<Result<TransactionResponse>>    CreateTransactionAsync(int userId, CreateTransactionRequest request, CancellationToken ct = default);
    Task<Result<TransactionResponse>>    DeleteTransactionAsync(int userId, int transactionId, CancellationToken ct = default);
    Task<Result<GoalResponse>>           UpsertGoalAsync(int userId, UpsertGoalRequest request, CancellationToken ct = default);
}
