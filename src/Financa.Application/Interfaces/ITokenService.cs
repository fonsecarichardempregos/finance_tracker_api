using Financa.Domain.Entities;

namespace Financa.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    int ExpiresInSeconds { get; }
}
