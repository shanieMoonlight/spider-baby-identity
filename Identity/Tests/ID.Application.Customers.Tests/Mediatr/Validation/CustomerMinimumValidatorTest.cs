// Concrete implementation of ACustomerMinimumValidator for testing
using FluentValidation.TestHelper;
using ID.Application.Customers.Mediatr.Validation;

namespace ID.Application.Customers.Tests.Mediatr.Validation;

public class CustomerMinimumValidatorTest : ACustomerMinimumValidator<TestRequest>
{
    public CustomerMinimumValidatorTest() : base() { }
    public CustomerMinimumValidatorTest(int position) : base(position) { }
}

// Unit tests for CustomerMinimumValidatorTest
public class CustomerMinimumValidatorTests
{
    private readonly CustomerMinimumValidatorTest _validator;
    private readonly CustomerMinimumValidatorTest _validatorWithPosition;

    public CustomerMinimumValidatorTests()
    {
        _validator = new CustomerMinimumValidatorTest();
        _validatorWithPosition = new CustomerMinimumValidatorTest(2);
    }

    //-----------------------------------//

    [Fact]
    public void Should_Have_Error_When_IsCustomerMinimum_Is_False()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = false,
            IsMntc = false,
            IsSuper = false
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IsCustomerMinimum)
              .WithErrorMessage("Forbidden!");
    }

    //-----------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_IsCustomerMinimum_Is_True()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsCustomerMinimum);
    }

    //-----------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_IsCustomerMinimum_Is_True_Mntc()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = false,
            IsMntc = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsCustomerMinimum);
    }

    //-----------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_IsCustomerMinimum_Is_True_Super()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = false,
            IsMntc = false,
            IsSuper = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsCustomerMinimum);
    }

    //-----------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Not_Authenticated()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = false,
            IsCustomer = false,
            IsMntc = false,
            IsSuper = false
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsCustomerMinimum);
    }

    //-----------------------------------//

    [Fact]
    public void Should_Have_Error_When_PrincipalTeamPosition_Is_Less_Than_Position()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = true,
            PrincipalTeamPosition = 1
        };

        // Act
        var result = _validatorWithPosition.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PrincipalTeamPosition)
              .WithErrorMessage("Forbidden!");
    }

    //-----------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_PrincipalTeamPosition_Is_Greater_Than_Or_Equal_To_Position()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = true,
            PrincipalTeamPosition = 2
        };

        // Act
        var result = _validatorWithPosition.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PrincipalTeamPosition);
    }

    //-----------------------------------//
}
