//using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
using ID.Application.Customers.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.AddCustomer;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
/// <summary>
/// Tests for non auth-validation
/// </summary>
public class AddCustomerMemberCmdValidatorTests
{
    private readonly AddCustomerMemberCmdValidator _validator;

    //- - - - - - - - - - - - - - - - - - //

    public AddCustomerMemberCmdValidatorTests()
    {
        _validator = new AddCustomerMemberCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenDtoIsNull()
    {
        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd(null));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenEmailIsEmpty()
    {
        // Arrange
        var dto = new AddCustomerMemberDto { Email = string.Empty };

        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd(dto));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void ShouldNotHaveValidationErrorWhenValidDataProvided()
    {
        // Arrange
        var dto = new AddCustomerMemberDto { Email = "test@example.com" };

        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd(dto));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new AddCustomerMemberCmdValidator();

        // Act & Assert
        Assert.IsType<ACustomerMinimumValidator<AddCustomerMemberCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<ACustomerMinimumValidator<AddCustomerMemberCmd>>();
    }

    //------------------------------------//

}//Cls
