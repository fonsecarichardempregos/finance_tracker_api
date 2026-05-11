namespace Financa.Domain.Entities;

public class MonthlyGoal
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public decimal TargetAmount { get; private set; }
    public short Month { get; private set; }
    public short Year { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private MonthlyGoal() { }

    public static MonthlyGoal Create(int userId, decimal targetAmount, short month, short year)
    {
        return new MonthlyGoal
        {
            UserId       = userId,
            TargetAmount = targetAmount,
            Month        = month,
            Year         = year,
            CreatedAt    = DateTime.UtcNow
        };
    }

    public void UpdateTarget(decimal targetAmount)
    {
        TargetAmount = targetAmount;
        UpdatedAt    = DateTime.UtcNow;
    }
}
