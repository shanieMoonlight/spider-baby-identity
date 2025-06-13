using ClArch.ValueObjects;
using ID.Domain.Entities.Teams;

namespace ID.Application.Features.Teams;

public static class TeamMappings
{

    public static Team Update(this Team model, TeamDto dto)
    {

        model.Update(
            Name.Create(dto.Name),
            DescriptionNullable.Create(dto.Description)
        );

        return model;

    }

    //------------------------------//

    public static TeamDto ToDto(this Team team) => new(team);

    //------------------------------//

    public static IEnumerable<TeamDto> ToDtos(this IEnumerable<Team> teams) =>
        teams.Select(t => t.ToDto());


}//Cls

