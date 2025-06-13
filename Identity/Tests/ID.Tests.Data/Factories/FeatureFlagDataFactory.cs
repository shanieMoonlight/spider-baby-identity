using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using MassTransit;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class FeatureFlagDataFactory
{

    public static List<FeatureFlag> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static FeatureFlag Create(
        Guid? id = null,
        string? name = null,
        string? description = null,
        string? administratorUsername = null,
        string? administratorId = null
        )
    {

        name ??= $"{RandomStringGenerator.Word()}";
        description ??= $"{RandomStringGenerator.Sentence()})";
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

        return ConstructorInvoker.CreateNoParamsInstance<FeatureFlag>(paramaters);
    }

    //------------------------------------//

}//Cls

