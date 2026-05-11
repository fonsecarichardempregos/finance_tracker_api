namespace Financa.Domain.Entities;

public class Transaction
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateOnly Date { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Category? Category { get; private set; }

    private Transaction() { }

    public static Transaction Create(
        int userId,
        int categoryId,
        decimal amount,
        string type,
        DateOnly date,
        string? description = null)
    {
        return new Transaction
        {
            UserId      = userId,
            CategoryId  = categoryId,
            Amount      = amount,
            Type        = type,
            Date        = date,
            Description = description,
            CreatedAt   = DateTime.UtcNow
        };
    }

    public void Update(int categoryId, decimal amount, DateOnly date, string? description)
    {
        CategoryId  = categoryId;
        Amount      = amount;
        Date        = date;
        Description = description;
        UpdatedAt   = DateTime.UtcNow;
    }
}
