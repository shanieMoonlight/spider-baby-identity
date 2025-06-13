using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class RefreshTokenDataFactory
{
    //------------------------------------//

    public static List<IdRefreshToken> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static IdRefreshToken Create(
            Guid? id = null,
            Guid? userId = null,
            string? payload = null,
            DateTime? expiresOnUtc = null,
            AppUser? user = null,
        string? administratorUsername = null,
        string? administratorId = null)
    {

        id ??= NewId.NextSequentialGuid();
        payload ??= $"{RandomStringGenerator.Generate(20)}{id}";
        expiresOnUtc ??= RandomDateGenerator.Generate(DateTime.Now.AddDays(5));

        if (user is not null)
            userId ??= user.Id;
        else
            userId ??= NewId.NextSequentialGuid();

        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";


        var paramaters = new[]
           {
            new PropertyAssignment(nameof(IdRefreshToken.Payload),  () => payload ),
            new PropertyAssignment(nameof(IdRefreshToken.ExpiresOnUtc),  () => expiresOnUtc ),
            new PropertyAssignment(nameof(IdRefreshToken.User),  () => user ),
            new PropertyAssignment(nameof(IdRefreshToken.UserId),  () => userId ),
            new PropertyAssignment(nameof(IdRefreshToken.Id),  () => id ),
            new PropertyAssignment(nameof(IdRefreshToken.AdministratorUsername),  () => administratorUsername ),
            new PropertyAssignment(nameof(IdRefreshToken.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<IdRefreshToken>(paramaters);
    }

    //------------------------------------//

}//Cls

