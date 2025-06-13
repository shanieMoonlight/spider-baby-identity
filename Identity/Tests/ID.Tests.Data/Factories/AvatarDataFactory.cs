using ID.Domain.Entities.Avatars;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class AvatarDataFactory
{

    public static List<Avatar> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static Avatar Create(
          Guid? id = null,
        string? b64 = null,
        string? url = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        b64 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        url ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(Avatar.B64),  () => b64 ),
        new PropertyAssignment(nameof(Avatar.Url),  () => url ),
        new PropertyAssignment(nameof(Avatar.Id),  () => id ),
        new PropertyAssignment(nameof(Avatar.AdministratorUsername),  () => administratorUsername ),
        new PropertyAssignment(nameof(Avatar.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<Avatar>(paramaters);
    }

    //------------------------------------//

}//Cls

