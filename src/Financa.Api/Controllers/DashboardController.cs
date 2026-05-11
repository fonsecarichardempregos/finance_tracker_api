using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Financa.Application.Interfaces;
using Financa.Contracts.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financa.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
[Produces("application/json")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    private int? CurrentUserId
    {
        get
        {
            var claim = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");
            return int.TryParse(claim, out var id) ? id : null;
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken ct)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var now = DateTime.UtcNow;
        var result = await dashboardService.GetDashboardAsync(
            CurrentUserId.Value,
            month ?? now.Month,
            year  ?? now.Year,
            ct);

        return result.IsFailure
            ? BadRequest(new { result.ErrorCode, result.Error })
            : Ok(result.Value);
    }

    [HttpPost("transactions")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTransaction(
        [FromBody] CreateTransactionRequest request,
        CancellationToken ct)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var result = await dashboardService.CreateTransactionAsync(CurrentUserId.Value, request, ct);

        return result.IsFailure
            ? BadRequest(new { result.ErrorCode, result.Error })
            : StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpDelete("transactions/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTransaction(int id, CancellationToken ct)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var result = await dashboardService.DeleteTransactionAsync(CurrentUserId.Value, id, ct);

        return result.IsFailure
            ? NotFound(new { result.ErrorCode, result.Error })
            : NoContent();
    }

    [HttpPost("goals")]
    [ProducesResponseType(typeof(GoalResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertGoal(
        [FromBody] UpsertGoalRequest request,
        CancellationToken ct)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var result = await dashboardService.UpsertGoalAsync(CurrentUserId.Value, request, ct);

        return result.IsFailure
            ? BadRequest(new { result.ErrorCode, result.Error })
            : Ok(result.Value);
    }
}
