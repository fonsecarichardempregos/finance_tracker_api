using System.Security.Claims;
using Financa.Application.Interfaces;
using Financa.Contracts.Auth;
using Financa.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financa.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // ─────────────────────────────────────────
    //  POST /api/auth/login
    // ─────────────────────────────────────────
    /// <summary>Autentica o usuário e retorna um token JWT.</summary>
    /// <response code="200">Login realizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Credenciais incorretas ou conta inativa.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);

        if (result.IsFailure)
        {
            var status = result.ErrorCode switch
            {
                ErrorCodes.InvalidCredentials => StatusCodes.Status401Unauthorized,
                ErrorCodes.UserInactive       => StatusCodes.Status401Unauthorized,
                _                             => StatusCodes.Status400BadRequest
            };
            return StatusCode(status, new ApiErrorResponse(result.ErrorCode!, result.Error!));
        }

        return Ok(result.Value);
    }

    // ─────────────────────────────────────────
    //  POST /api/auth/register
    // ─────────────────────────────────────────
    /// <summary>Cria uma nova conta de usuário.</summary>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="409">E-mail já cadastrado.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct)
    {
        var result = await authService.RegisterAsync(request, ct);

        if (result.IsFailure)
        {
            var status = result.ErrorCode switch
            {
                ErrorCodes.EmailAlreadyExists => StatusCodes.Status409Conflict,
                _                             => StatusCodes.Status400BadRequest
            };
            return StatusCode(status, new ApiErrorResponse(result.ErrorCode!, result.Error!));
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    // ─────────────────────────────────────────
    //  PUT /api/auth/change-password
    // ─────────────────────────────────────────
    /// <summary>Altera a senha do usuário autenticado.</summary>
    /// <response code="200">Senha alterada com sucesso.</response>
    /// <response code="400">Senhas não coincidem ou dados inválidos.</response>
    /// <response code="401">Não autenticado ou senha atual incorreta.</response>
    [HttpPut("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ChangePasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        // Extrai o userId do token JWT (claim "sub")
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? User.FindFirstValue("sub");

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new ApiErrorResponse(
                ErrorCodes.Unauthorized, "Token inválido."));

        var result = await authService.ChangePasswordAsync(userId, request, ct);

        if (result.IsFailure)
        {
            var status = result.ErrorCode switch
            {
                ErrorCodes.InvalidCurrentPass => StatusCodes.Status401Unauthorized,
                ErrorCodes.PasswordMismatch   => StatusCodes.Status400BadRequest,
                ErrorCodes.UserNotFound       => StatusCodes.Status404NotFound,
                _                             => StatusCodes.Status400BadRequest
            };
            return StatusCode(status, new ApiErrorResponse(result.ErrorCode!, result.Error!));
        }

        return Ok(result.Value);
    }
}

// ── Shared error response ─────────────────────
public record ApiErrorResponse(string Code, string Message);
