using ID.Domain.Entities.AppUsers.OAuth;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class OAuthInfoDataFactory
{


    public static List<OAuthInfo> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static OAuthInfo Create(
          Guid? id = null,
        string? issuer = null,
        string? imageUrl = null,
        Guid? appUserId = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        issuer ??= $"{RandomStringGenerator.Generate(20)}{id}";
        imageUrl ??= $"{RandomStringGenerator.Generate(20)}{id}";
        appUserId ??= NewId.NextSequentialGuid();
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(OAuthInfo.Issuer),  () => issuer ),
        new PropertyAssignment(nameof(OAuthInfo.ImageUrl),  () => imageUrl ),
        new PropertyAssignment(nameof(OAuthInfo.AppUserId),  () => appUserId ),
        new PropertyAssignment(nameof(OAuthInfo.Id),  () => id ),
        new PropertyAssignment(nameof(OAuthInfo.AdministratorUsername),  () => administratorUsername ),
        new PropertyAssignment(nameof(OAuthInfo.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<OAuthInfo>(paramaters);
    }

    //------------------------------------//

}//Cls

