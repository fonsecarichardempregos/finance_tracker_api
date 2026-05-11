using System.ComponentModel.DataAnnotations;

namespace Financa.Contracts.Auth;

// ── Request ───────────────────────────────────
public record LoginRequest(
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    string Password
);

// ── Response ──────────────────────────────────
public record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,        // segundos
    UserDto User
);

public record UserDto(
    int Id,
    string FullName,
    string Email,
    string? Phone,
    DateOnly? BirthDate,
    DateTime CreatedAt
);
