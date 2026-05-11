namespace Financa.Domain.Entities;

public class PasswordResetCode
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public User? User { get; private set; }


    private PasswordResetCode() { }

    public static PasswordResetCode Create(int userId, string code, int expiresInMinutes = 15)
    {
        return new PasswordResetCode
        {
            UserId = userId,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsValid() => !IsUsed && DateTime.UtcNow < ExpiresAt;

    public void MarkAsUsed() => IsUsed = true;
}
