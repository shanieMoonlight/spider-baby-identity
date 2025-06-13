using ID.Application.Features.SubscriptionPlans.Qry.GetAll;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetAll;

public class GetAllSubscriptionPlansQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        var validator = new GetAllSubscriptionPlansQryValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetAllSubscriptionPlansQry>>();
    }

    //------------------------------------//


}//Cls