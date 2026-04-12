using HelpMe.Domain.Entities;

namespace HelpMe.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
