using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using System.Security.Claims;

namespace ID.Application.AppAbs.ExtraClaims;

//========================================================//

public interface IExtraClaimsGenerator
{
    IEnumerable<Claim> Generate(AppUser user, Team team);
}

//========================================================//

/// <summary>
/// Placeholder that return an Empty array
/// </summary>
public class DefaultExtraClaimsGenerator : IExtraClaimsGenerator
{
    public IEnumerable<Claim> Generate(AppUser user, Team team) => [];
}


//========================================================//