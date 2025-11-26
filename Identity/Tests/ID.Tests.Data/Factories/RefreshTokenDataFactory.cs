using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.TrustedDevices;

namespace ID.Tests.Data.Factories;

public static class RefreshTokenDataFactory
{
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
            IEnumerable<AuthMethodRef>? authMethodRefs = null,
            Guid? trustedDeviceId = null,
            TrustedDevice? trustedDevice = null,
            string? administratorUsername = null,
            string? administratorId = null)
    {

        id ??= Guid.NewGuid();
        payload ??= $"{RandomStringGenerator.Generate(20)}{id}";
        expiresOnUtc ??= RandomDateGenerator.Generate(DateTime.Now.AddDays(5));
        authMethodRefs ??= [];

        if (user is not null)
            userId ??= user.Id;
        else
            userId ??= Guid.NewGuid();

        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        trustedDeviceId = trustedDevice?.Id ?? trustedDeviceId;


        var paramaters = new[]
           {
            new PropertyAssignment(nameof(IdRefreshToken.PayloadHash),  () => payload ),
            new PropertyAssignment(nameof(IdRefreshToken.ExpiresOnUtc),  () => expiresOnUtc ),
            new PropertyAssignment(nameof(IdRefreshToken.User),  () => user ),
            new PropertyAssignment(nameof(IdRefreshToken.UserId),  () => userId ),
            new PropertyAssignment(nameof(IdRefreshToken.Id),  () => id ),
            new PropertyAssignment(nameof(IdRefreshToken.TrustedDeviceId),  () => trustedDeviceId ),
            new PropertyAssignment(nameof(IdRefreshToken.TrustedDevice),  () => trustedDevice ),
            new PropertyAssignment(nameof(IdRefreshToken.AuthMethodRefs),  () => authMethodRefs.ToList() ),
            new PropertyAssignment(nameof(IdRefreshToken.AdministratorUsername),  () => administratorUsername ),
            new PropertyAssignment(nameof(IdRefreshToken.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<IdRefreshToken>(paramaters);
    }

    //------------------------------------//

}//Cls

