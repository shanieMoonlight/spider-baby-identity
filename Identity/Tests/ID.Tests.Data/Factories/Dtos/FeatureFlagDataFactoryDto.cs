using MassTransit;
using TestingHelpers;
using ID.Application.Features.FeatureFlags;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Tests.Data.Factories.Dtos;

public static class FeatureFlagDtoDataFactory
{

    public static List<FeatureFlagDto> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static FeatureFlagDto Create(
          Guid? id = null,
        string? name = null,
        string? description = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        name ??= $"{RandomStringGenerator.Generate(20)}{id}";
        description ??= $"{RandomStringGenerator.Generate(20)}{id}";
        id ??= NewId.NextSequentialGuid();
        administratorUsername ??= $"{RandomStringGenerator.Generate(20)}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";

        var paramaters = new[]
           {
               new PropertyAssignment(nameof(FeatureFlag.Name),  () => name ),
        new PropertyAssignment(nameof(FeatureFlag.Description),  () => description ),
        new PropertyAssignment(nameof(FeatureFlag.Id),  () => id ),
        new PropertyAssignment(nameof(FeatureFlag.AdministratorUsername),  () => administratorUsername ),
        new PropertyAssignment(nameof(FeatureFlag.AdministratorId),  () => administratorId )
        };


        return ConstructorInvoker.CreateNoParamsInstance<FeatureFlagDto>(paramaters);

    }

    //------------------------------------//

}//Cls

