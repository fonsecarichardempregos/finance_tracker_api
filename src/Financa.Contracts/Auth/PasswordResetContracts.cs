using System.ComponentModel.DataAnnotations;

namespace Financa.Contracts.Auth;

public record ForgotPasswordRequest(
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email
);

public record ForgotPasswordResponse(
    string Message
);

public record VerifyResetCodeRequest(
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "O código deve ter 6 dígitos")]
    string Code
);

public record VerifyResetCodeResponse(
    string Message,
    string ResetToken
);

public record ResetPasswordRequest(
    [Required(ErrorMessage = "Token é obrigatório")]
    string ResetToken,

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    string NewPassword,

    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    string ConfirmNewPassword
);

public record ResetPasswordResponse(
    string Message
);
