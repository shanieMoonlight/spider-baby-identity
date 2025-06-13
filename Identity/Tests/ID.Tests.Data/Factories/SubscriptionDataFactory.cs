using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.Teams;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class SubscriptionDataFactory
{

    public static List<TeamSubscription> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static TeamSubscription Create(
        Guid? id = null,
        double? discount = null,
        double? lastPaymenAmount = null,
        Guid? teamId = null,
        Guid? subscriptionPlanId = null,
        bool? trial = null,
        int? deviceLimit = null,
        HashSet<TeamDevice>? devices = null,
        string? name = null,
        string? description = null,
        SubscriptionRenewalTypes? renewalType = null,
        SubscriptionPlan? plan = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        discount ??= 0;
        lastPaymenAmount ??= 0;
        teamId ??= NewId.NextSequentialGuid();
        subscriptionPlanId ??= NewId.NextSequentialGuid();
        trial ??= false;
        name ??= $"{RandomStringGenerator.Generate(20)}{id}";
        description ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        deviceLimit ??= 0;
        devices ??= [];
        renewalType ??= MyRandomDataGenerator.Enum<SubscriptionRenewalTypes>();

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(TeamSubscription.Discount),  () => discount ),
                new PropertyAssignment(nameof(TeamSubscription.LastPaymenAmount),  () => lastPaymenAmount ),
                new PropertyAssignment(nameof(TeamSubscription.TeamId),  () => teamId ),
                new PropertyAssignment(nameof(TeamSubscription.SubscriptionPlanId),  () => plan?.Id ?? subscriptionPlanId ),
                new PropertyAssignment(nameof(TeamSubscription.SubscriptionPlan),  () => plan ),
                new PropertyAssignment(nameof(TeamSubscription.Trial),  () => trial ),
                new PropertyAssignment(nameof(TeamSubscription.Name),  () => name ),
                new PropertyAssignment(nameof(TeamSubscription.Description),  () => description ),
                new PropertyAssignment(nameof(TeamSubscription.DeviceLimit),  () => deviceLimit ),
                new PropertyAssignment(nameof(TeamSubscription.RenewalType),  () => renewalType ),
                new PropertyAssignment(nameof(TeamSubscription.Devices),  () => devices ),
                new PropertyAssignment(nameof(TeamSubscription.Id),  () => id ),
                new PropertyAssignment(nameof(TeamSubscription.StartDate),  () => startDate ),
                new PropertyAssignment(nameof(TeamSubscription.EndDate),  () => endDate ),
                new PropertyAssignment(nameof(TeamSubscription.AdministratorUsername),  () => administratorUsername ),
                new PropertyAssignment(nameof(TeamSubscription.AdministratorId),  () => administratorId )
        };


        var sub = ConstructorInvoker.CreateNoParamsInstance<TeamSubscription>(paramaters);

        NonPublicClassMembers.SetField(sub, "_devices", devices);

        return sub;
    }

    //------------------------------------//

}//Cls

