using System.ComponentModel.DataAnnotations;

namespace Financa.Contracts.Auth;

// ─────────────────────────────────────────────
//  CADASTRO
// ─────────────────────────────────────────────

public record RegisterRequest(
    [Required(ErrorMessage = "Nome completo é obrigatório")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
    string FullName,

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    string Password,

    string? Phone,

    DateOnly? BirthDate
);

public record RegisterResponse(
    string Message,
    UserDto User
);

// ─────────────────────────────────────────────
//  TROCA DE SENHA
// ─────────────────────────────────────────────

public record ChangePasswordRequest(
    [Required(ErrorMessage = "Senha atual é obrigatória")]
    string CurrentPassword,

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Nova senha deve ter no mínimo 6 caracteres")]
    string NewPassword,

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    string ConfirmNewPassword
);

public record ChangePasswordResponse(
    string Message
);
