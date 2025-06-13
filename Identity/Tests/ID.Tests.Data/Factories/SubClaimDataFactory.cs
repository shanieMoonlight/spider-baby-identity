using ID.Application.JWT.Subscriptions;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class SubscriptionClaimDataFactory
{
    //------------------------------------//

    public static List<SubscriptionClaimData> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create())];
    }

    //- - - - - - - - - - - - - - - - - - -//

    public static SubscriptionClaimData Create(
            string? name = null,
            bool? trial = null,
            string? status = null,
            string? expiry = null,
            string? deviceId = null)
    {

        name ??= $"{RandomStringGenerator.Generate(10)}";
        expiry ??= $"{RandomStringGenerator.Generate(10)}";
        trial ??= RandomBooleanGenerator.Generate();
        status ??= $"{RandomStringGenerator.Generate(10)}";
        deviceId ??= $"{RandomStringGenerator.Generate(10)}";

        var paramaters = new[]
           {
            new PropertyAssignment(nameof(SubscriptionClaimData.Name),  () => name ),
            new PropertyAssignment(nameof(SubscriptionClaimData.Trial),  () => trial ),
            new PropertyAssignment(nameof(SubscriptionClaimData.Status),  () => status ),
            new PropertyAssignment(nameof(SubscriptionClaimData.Expiry),  () => expiry ),
            new PropertyAssignment(nameof(SubscriptionClaimData.DeviceId),  () => deviceId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<SubscriptionClaimData>(paramaters);
    }

    //------------------------------------//

}//Cls

