using FluentValidation.TestHelper;
using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomer;
using ID.Application.Customers.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Qry.GetCustomer;

public class GetCustomerQryValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenAllIsNull()
    {
        // Arrange
        var validator = new GetCustomerQryValidator();
        var command = new GetCustomerQry(default, default);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TeamId);
        result.ShouldHaveValidationErrorFor(x => x.MemberId);
        result.IsValid.ShouldBeFalse();

    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoTeamID()
    {
        // Arrange
        var validator = new GetCustomerQryValidator();
        var command = new GetCustomerQry(default, Guid.NewGuid());
        command.SetAuthenticated_MNTC();
        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoSubscriptionPlanId()
    {
        // Arrange
        var validator = new GetCustomerQryValidator();
        var command = new GetCustomerQry(Guid.NewGuid(), default);
        command.SetAuthenticated_MNTC();
        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid()
    {
        // Arrange
        var validator = new GetCustomerQryValidator();
        var command = new GetCustomerQry(Guid.NewGuid(), Guid.NewGuid());
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetCustomerQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetCustomerQry>>();
    }

    //------------------------------------//

}//Cls