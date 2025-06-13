using ID.Domain.Entities.SubscriptionPlans;
using ID.Tests.Data.Factories;

namespace ID.Tests.Data.Initializers;
public class SubscriptionPlansInitializer
{
    //--------------------------------//

    public static List<SubscriptionPlan> GenerateData(int count = 25)
    {
        return SubscriptionPlanDataFactory.CreateMany(count);
    }

    //--------------------------------//

}//Cls
