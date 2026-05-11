using System.ComponentModel.DataAnnotations;

namespace Financa.Contracts.Dashboard;

public record DashboardResponse(
    string        Month,
    decimal       Balance,
    decimal       MonthIncome,
    decimal       MonthExpense,
    decimal?      GoalTarget,
    double?       GoalProgressPercent,
    List<CategoryExpenseDto> ExpensesByCategory,
    List<RecentTransactionDto> RecentTransactions
);

public record CategoryExpenseDto(
    int     CategoryId,
    string  Name,
    string  Icon,
    string  Color,
    decimal Amount,
    double  Percent
);

public record RecentTransactionDto(
    int     Id,
    string  Name,
    string  Category,
    string  Icon,
    string  Color,
    decimal Amount,
    string  Type,
    string  Date
);

public record CreateTransactionRequest(
    [Required(ErrorMessage = "Categoria é obrigatória")]
    int CategoryId,

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    decimal Amount,

    [Required(ErrorMessage = "Tipo é obrigatório")]
    string Type,

    [Required(ErrorMessage = "Data é obrigatória")]
    DateOnly Date,

    string? Description
);

public record TransactionResponse(
    int      Id,
    int      CategoryId,
    string   CategoryName,
    string   CategoryIcon,
    string   CategoryColor,
    decimal  Amount,
    string   Type,
    string?  Description,
    DateOnly Date,
    DateTime CreatedAt
);

public record UpsertGoalRequest(
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Meta deve ser maior que zero")]
    decimal TargetAmount,

    [Required]
    [Range(1, 12)]
    short Month,

    [Required]
    [Range(2020, 2100)]
    short Year
);

public record GoalResponse(
    int     Id,
    decimal TargetAmount,
    short   Month,
    short   Year
);
