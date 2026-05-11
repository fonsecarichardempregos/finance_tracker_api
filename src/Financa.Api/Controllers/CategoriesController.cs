using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Financa.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financa.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
[Produces("application/json")]
public class CategoriesController(ICategoryRepository categoryRepository) : ControllerBase
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
    public async Task<IActionResult> GetCategories(
        [FromQuery] string? type,
        CancellationToken ct)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var categories = await categoryRepository.GetByUserAsync(CurrentUserId.Value, type, ct);

        var result = categories.Select(c => new
        {
            c.Id,
            c.Name,
            c.Icon,
            c.Color,
            c.Type
        });

        return Ok(result);
    }
}
