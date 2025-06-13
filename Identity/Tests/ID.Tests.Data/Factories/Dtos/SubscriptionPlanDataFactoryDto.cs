using ID.Application.Features.FeatureFlags;
using ID.Application.Features.SubscriptionPlans;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories.Dtos;

public static class SubscriptionPlanDtoDataFactory
{

    public static List<SubscriptionPlanDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static SubscriptionPlanDto Create(
          Guid? id = null,
        string? name = null,
        string? description = null,
        int? deviceLimit = null,
        int? trialMonths = null,
        double? price = null,
        IEnumerable<FeatureFlagDto>? featureFlags = null,
        IEnumerable<Guid>? featureFlagIds = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {
        featureFlags ??= [];
        featureFlagIds ??= [];
        name ??= $"{RandomStringGenerator.Generate(20)}{id}";
        description ??= $"{RandomStringGenerator.Generate(20)}{id}";
        deviceLimit ??= 0;
        trialMonths ??= 0;
        price ??= 0;
        id ??= NewId.NextSequentialGuid();


        var paramaters = new[]
           {
                new PropertyAssignment(nameof(SubscriptionPlanDto.Name),  () => name ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.Description),  () => description ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.DeviceLimit),  () => deviceLimit ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.TrialMonths),  () => trialMonths ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.Price),  () => price ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.Id),  () => id ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.FeatureFlags),  () => featureFlags.ToList() ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.FeatureFlagIds),  () => featureFlagIds.ToList() ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.AdministratorUsername),  () => administratorUsername ),
                new PropertyAssignment(nameof(SubscriptionPlanDto.AdministratorId),  () => administratorId ),
            };

        return ConstructorInvoker.CreateNoParamsInstance<SubscriptionPlanDto>(paramaters);

    }

    //------------------------------------//

}//Cls

