using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Domain.Entities.Teams;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class DeviceDtoDataFactory
{

    //------------------------------------//

    public static List<DeviceDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static DeviceDto Create(
          Guid? id = null,
        string? name = null,
        string? description = null,
        string? uniqueId = null,
        Guid? subscriptionId = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        name ??= $"{RandomStringGenerator.Generate(20)}{id}";
        description ??= $"{RandomStringGenerator.Generate(20)}{id}";
        uniqueId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        subscriptionId ??= NewId.NextSequentialGuid();
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(TeamDevice.Name),  () => name ),
        new PropertyAssignment(nameof(TeamDevice.Description),  () => description ),
        new PropertyAssignment(nameof(TeamDevice.UniqueId),  () => uniqueId ),
        new PropertyAssignment(nameof(TeamDevice.SubscriptionId),  () => subscriptionId ),
        new PropertyAssignment(nameof(TeamDevice.Id),  () => id ),
        new PropertyAssignment(nameof(TeamDevice.AdministratorUsername),  () => administratorUsername ),
        new PropertyAssignment(nameof(TeamDevice.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<DeviceDto>(paramaters);
    }

    //------------------------------------//

}//Cls

