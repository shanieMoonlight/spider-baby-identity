using ID.Domain.Entities.Teams;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.Teams;
public class GetTeamByNameSpec : ASimpleSpecification<Team> 
{
    internal GetTeamByNameSpec(string? name) : base(f => f.Name == name)
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}
