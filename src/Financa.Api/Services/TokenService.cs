using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Financa.Application.Interfaces;
using Financa.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Financa.Api.Services;

public class TokenService : ITokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiresInSeconds;

    public int ExpiresInSeconds => _expiresInSeconds;

    public TokenService(IConfiguration configuration)
    {
        _secretKey  = configuration["Jwt:SecretKey"]  ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");
        _issuer     = configuration["Jwt:Issuer"]     ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
        _audience   = configuration["Jwt:Audience"]   ?? throw new InvalidOperationException("Jwt:Audience não configurado.");
        _expiresInSeconds = int.Parse(configuration["Jwt:ExpiresInSeconds"] ?? "86400");
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_expiresInSeconds),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
