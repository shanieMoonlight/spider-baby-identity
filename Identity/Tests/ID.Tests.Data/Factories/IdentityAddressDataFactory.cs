using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class IdentityAddressDataFactory
{

    //------------------------------------//

    public static List<IdentityAddress> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static IdentityAddress Create(
        Guid appUserId,
        Guid? id = null,
        string? line1 = null,
        string? line2 = null,
        string? line3 = null,
        string? line4 = null,
        string? line5 = null,
        string? eirCode = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        line1 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line2 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line3 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line4 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        line5 ??= $"{RandomStringGenerator.Generate(20)}{id}";
        eirCode ??= $"{RandomStringGenerator.Generate(AreaCodeNullable.MaxLength)}{id}";

        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
                new PropertyAssignment(nameof(IdentityAddress.Line1),  () => line1 ),
                new PropertyAssignment(nameof(IdentityAddress.Line2),  () => line2 ),
                new PropertyAssignment(nameof(IdentityAddress.Line3),  () => line3 ),
                new PropertyAssignment(nameof(IdentityAddress.Line4),  () => line4 ),
                new PropertyAssignment(nameof(IdentityAddress.Line5),  () => line5 ),
                new PropertyAssignment(nameof(IdentityAddress.AreaCode),  () => eirCode ),
                new PropertyAssignment(nameof(IdentityAddress.AppUserId),  () => appUserId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<IdentityAddress>(paramaters);
    }

    //------------------------------------//

}//Cls

