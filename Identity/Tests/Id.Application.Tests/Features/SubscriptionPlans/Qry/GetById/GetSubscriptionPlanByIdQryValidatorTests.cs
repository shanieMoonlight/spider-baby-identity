using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans.Qry.GetById;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetById;

public class GetSubscriptionPlanByIdQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new GetSubscriptionPlanByIdQryValidator();
        var command = new GetSubscriptionPlanByIdQry(null);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        var validator = new GetSubscriptionPlanByIdQryValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetSubscriptionPlanByIdQry>>();
    }

    //------------------------------------//


}//Cls