using ID.Application.Features.Teams;
using ID.Domain.Entities.Teams;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class TeamDtoDataFactory
{

    //------------------------------------//

    public static List<TeamDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static TeamDto Create(
          Guid? id = null,
        string? name = null,
        string? description = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        name ??= $"{RandomStringGenerator.Generate(20)}{id}";
        description ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(Team.Name),  () => name ),
        new PropertyAssignment(nameof(Team.Description),  () => description ),
        new PropertyAssignment(nameof(Team.Id),  () => id ),
        new PropertyAssignment(nameof(Team.AdministratorUsername),  () => administratorUsername ),
        new PropertyAssignment(nameof(Team.AdministratorId),  () => administratorId )
        };

        return ConstructorInvoker.CreateNoParamsInstance<TeamDto>(paramaters);
    }

    //------------------------------------//

}//Cls

