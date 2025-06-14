using ID.OAuth.Google.Features.GoogleCookieSignUp;
using ID.OAuth.Google.Features.GoogleSignIn;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class GoogleSignInDtoFactory
{

    //------------------------------------//

    public static List<GoogleSignInDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create())];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static GoogleSignInDto Create(
      string? idToken = null,
      Guid? subscriptionId = null,
      string? deviceId = null)
    {
        idToken ??= RandomStringGenerator.Generate(40);
        subscriptionId ??= NewId.NextSequentialGuid();
        deviceId ??= RandomStringGenerator.Generate(20);

        var parameters = new[]
        {
            new PropertyAssignment(nameof(GoogleSignInDto.IdToken), () => idToken),
            new PropertyAssignment(nameof(GoogleSignInDto.SubscriptionId), () => subscriptionId),
            new PropertyAssignment(nameof(GoogleSignInDto.DeviceId), () => deviceId)
        };

        return ConstructorInvoker.CreateNoParamsInstance<GoogleSignInDto>(parameters);
    }



}//Cls

