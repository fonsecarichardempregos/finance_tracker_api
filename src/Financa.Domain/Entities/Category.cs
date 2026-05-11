namespace Financa.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Category() { }

    public static Category Create(int userId, string name, string icon, string color, string type)
    {
        return new Category
        {
            UserId    = userId,
            Name      = name,
            Icon      = icon,
            Color     = color,
            Type      = type,
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
