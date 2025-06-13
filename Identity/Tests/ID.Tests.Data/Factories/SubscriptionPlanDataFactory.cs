using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class SubscriptionPlanDataFactory
{

    //------------------------------------//

    public static List<SubscriptionPlan> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static SubscriptionPlan Create(
          Guid? id = null,
        string? name = null,
        string? description = null,
        int? deviceLimit = null,
        int? trialMonths = null,
        double? price = null,
        SubscriptionRenewalTypes? renewalType = null,
        IEnumerable<FeatureFlag>? featureFlags = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        name ??= $"{RandomStringGenerator.Word()}";
        description ??= $"{RandomStringGenerator.Sentence()})";
        deviceLimit ??= MyRandomNumberGenerator.Integer(0, 50);
        trialMonths ??= MyRandomNumberGenerator.Integer(0, 12);
        price ??= MyRandomNumberGenerator.Double(0, 500);
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        renewalType ??= MyRandomDataGenerator.Enum<SubscriptionRenewalTypes>();
        featureFlags ??= [];

        var paramaters = new[]
            {
                new PropertyAssignment(nameof(SubscriptionPlan.Id),  () => id ),
                new PropertyAssignment(nameof(SubscriptionPlan.Name),  () => name ),
                new PropertyAssignment(nameof(SubscriptionPlan.Description),  () => description ),
                new PropertyAssignment(nameof(SubscriptionPlan.DeviceLimit),  () => deviceLimit ),
                new PropertyAssignment(nameof(SubscriptionPlan.TrialMonths),  () => trialMonths ),
                new PropertyAssignment(nameof(SubscriptionPlan.Price),  () => price ),
                new PropertyAssignment(nameof(SubscriptionPlan.RenewalType),  () => renewalType ),
                new PropertyAssignment(nameof(SubscriptionPlan.AdministratorId),  () => administratorId ),
                new PropertyAssignment(nameof(SubscriptionPlan.AdministratorUsername),  () => administratorUsername ),
                new PropertyAssignment("_featureFlags",  () => featureFlags.ToHashSet() )
            };


        return ConstructorInvoker.CreateNoParamsInstance<SubscriptionPlan>(paramaters);
    }

    //------------------------------------//

}//Cls

