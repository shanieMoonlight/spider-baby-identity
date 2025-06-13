using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans.Qry.GetByName;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetByName;

public class GetSubscriptionPlanByNameValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new GetSubscriptionPlanByNameQryValidator();
        var command = new GetSubscriptionPlanByNameQry(null);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        var validator = new GetSubscriptionPlanByNameQryValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetSubscriptionPlanByNameQry>>();
    }

    //------------------------------------//


}//Cls