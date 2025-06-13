using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Tests.Data.Factories;

namespace ID.Tests.Data.Initializers;
public class FeatureFlagInitializer
{
    //------------------------------------//

    public static List<FeatureFlag> GenerateData(int count = 50)
    {

        return FeatureFlagDataFactory.CreateMany(50);
    }

    //------------------------------------//

}//Cls
