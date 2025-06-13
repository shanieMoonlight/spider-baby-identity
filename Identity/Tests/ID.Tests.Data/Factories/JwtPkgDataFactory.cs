using ID.Domain.Models;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class JwtPackageDataFactory
{


    //- - - - - - - - - - - - - - - - - - //

    public static List<JwtPackage> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create())];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static JwtPackage Create(
        string? accessToken = null,
        long? expiration = null,
        bool? twoStepVerificationRequired = null,
        TwoFactorProvider? twoFactorProvider = null,
        string? extraInfo = null)
    {

        accessToken ??= $"{RandomStringGenerator.Generate(50)}";
        expiration ??= MyRandomNumberGenerator.Long(10000);
        twoStepVerificationRequired ??= RandomBooleanGenerator.Generate();
        extraInfo ??= $"{RandomStringGenerator.Sentence()}";
        twoFactorProvider ??= MyRandomDataGenerator.Enum<TwoFactorProvider>();


        var paramaters = new List<PropertyAssignment>()
           {
                new(nameof(JwtPackage.AccessToken),  () => accessToken ),
                new(nameof(JwtPackage.Expiration),  () => expiration ),
                new(nameof(JwtPackage.ExtraInfo),  () => extraInfo ),
                //new PropertyAssignment("_teamSubscriptions",  () => subscriptions ),
                new(nameof(JwtPackage.TwoFactorProvider),  () => twoFactorProvider ),
                new(nameof(JwtPackage.TwoStepVerificationRequired),  () => twoStepVerificationRequired ),
                new(nameof(JwtPackage.ExtraInfo),  () => extraInfo )

            };

        return ConstructorInvoker.CreateNoParamsInstance<JwtPackage>([.. paramaters]);

    }

    //------------------------------------//

}//Cls

