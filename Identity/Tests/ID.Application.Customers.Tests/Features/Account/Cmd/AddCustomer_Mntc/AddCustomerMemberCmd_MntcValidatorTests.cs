using FluentValidation.TestHelper;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;


namespace ID.Application.Customers.Tests.Features.Account.Cmd.AddCustomer_Mntc;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
/// <summary>
/// Tests for non auth-validation
/// </summary>
public class AddCustomerMemberCmd_MntcValidatorTests
{
    private readonly AddCustomerMemberCmd_MntcValidator _validator;

    //- - - - - - - - - - - - - - - - - - //

    public AddCustomerMemberCmd_MntcValidatorTests()
    {
        _validator = new AddCustomerMemberCmd_MntcValidator();
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenDtoIsNull()
    {
        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd_Mntc(null));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenEmailIsEmpty()
    {
        // Arrange
        var dto = new AddCustomerMember_MntcDto { Email = string.Empty };

        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd_Mntc(dto));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenTeamIdIsEmpty()
    {
        // Arrange
        var dto = new AddCustomerMember_MntcDto { TeamId = default };

        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd_Mntc(dto));

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.TeamId);
    }

    //------------------------------------//

    [Fact]
    public void ShouldNotHaveValidationErrorWhenValidDataProvided()
    {
        // Arrange
        var dto = new AddCustomerMember_MntcDto { Email = "test@example.com", TeamId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(new AddCustomerMemberCmd_Mntc(dto));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new AddCustomerMemberCmd_MntcValidator();

        // Act & Assert
        Assert.IsType<AMntcMinimumValidator<AddCustomerMemberCmd_Mntc>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<AddCustomerMemberCmd_Mntc>>();
    }

    //------------------------------------//

}//Cls
