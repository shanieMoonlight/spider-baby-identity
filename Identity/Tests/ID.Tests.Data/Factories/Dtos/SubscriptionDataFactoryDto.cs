using ID.Application.Features.FeatureFlags;
using ID.Application.Features.Teams;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class SubscriptionDtoDataFactory
{
    //------------------------------------//

    public static List<SubscriptionDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static SubscriptionDto Create(
          Guid? id = null,
        double? discount = null,
        double? lastPaymenAmount = null,
        Guid? teamId = null,
        Guid? subscriptionPlanId = null,
        bool? trial = null,
        string? name = null,
        string? description = null,
        string? administratorUsername = null,
        string? administratorId = null,
        IEnumerable<FeatureFlagDto>? featureFlags = null,
        IEnumerable<Guid>? featureFlagIds = null
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
        featureFlags ??= [];
        featureFlagIds ??= [];

        var paramaters = new[]
           {
            new PropertyAssignment(nameof(SubscriptionDto.Discount),  () => discount ),
            new PropertyAssignment(nameof(SubscriptionDto.LastPaymenAmount),  () => lastPaymenAmount ),
            new PropertyAssignment(nameof(SubscriptionDto.TeamId),  () => teamId ),
            new PropertyAssignment(nameof(SubscriptionDto.SubscriptionPlanId),  () => subscriptionPlanId ),
            new PropertyAssignment(nameof(SubscriptionDto.Trial),  () => trial ),
            new PropertyAssignment(nameof(SubscriptionDto.Name),  () => name ),
            new PropertyAssignment(nameof(SubscriptionDto.Description),  () => description ),
            new PropertyAssignment(nameof(SubscriptionDto.Id),  () => id ),
            new PropertyAssignment(nameof(SubscriptionDto.AdministratorUsername),  () => administratorUsername ),
            new PropertyAssignment(nameof(SubscriptionDto.AdministratorId),  () => administratorId ),
            new PropertyAssignment(nameof(SubscriptionDto.AdministratorId),  () => administratorId ),
        };


        return ConstructorInvoker.CreateNoParamsInstance<SubscriptionDto>(paramaters);
    }

    //------------------------------------//

}//Cls

