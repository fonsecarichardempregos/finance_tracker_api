using Financa.Application.Interfaces;
using Financa.Contracts.Auth;
using Financa.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Financa.Api.Controllers;

[ApiController]
[Route("api/auth/password-reset")]
[Produces("application/json")]
public class PasswordResetController(IPasswordResetService passwordResetService) : ControllerBase
{
    [HttpPost("request")]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestCode(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken ct)
    {
        var result = await passwordResetService.RequestCodeAsync(request, ct);

        if (result.IsFailure)
            return BadRequest(new ApiErrorResponse(result.ErrorCode!, result.Error!));

        return Ok(result.Value);
    }

    [HttpPost("verify")]
    [ProducesResponseType(typeof(VerifyResetCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyCode(
        [FromBody] VerifyResetCodeRequest request,
        CancellationToken ct)
    {
        var result = await passwordResetService.VerifyCodeAsync(request, ct);

        if (result.IsFailure)
            return BadRequest(new ApiErrorResponse(result.ErrorCode!, result.Error!));

        return Ok(result.Value);
    }

    [HttpPost("reset")]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        var result = await passwordResetService.ResetPasswordAsync(request, ct);

        if (result.IsFailure)
            return BadRequest(new ApiErrorResponse(result.ErrorCode!, result.Error!));

        return Ok(result.Value);
    }
}
