using ID.Application.Features.SubscriptionPlans.Qry.GetPage;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetPage;

public class GetSubscriptionPlanPageQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        var validator = new GetSubscriptionPlansPageQryValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetSubscriptionPlansPageQry>>();
    }

    //------------------------------------//


}//Cls