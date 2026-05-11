namespace Financa.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    // EF Core constructor
    private User() { }

    public static User Create(
        string fullName,
        string email,
        string passwordHash,
        string? phone = null,
        DateOnly? birthDate = null)
    {
        return new User
        {
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Phone = phone,
            BirthDate = birthDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateProfile(string fullName, string? phone, DateOnly? birthDate)
    {
        FullName = fullName.Trim();
        Phone = phone;
        BirthDate = birthDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
}
