// Concrete implementation of ACustomerOnlyValidator for testing
using FluentValidation.TestHelper;
using ID.Application.Customers.Mediatr.Validation;
using ID.Application.Mediatr.Cqrslmps;

namespace ID.Application.Customers.Tests.Mediatr.Validation;

public class CustomerOnlyValidatorTest : ACustomerOnlyValidator<TestRequest>
{
}

// Test request class implementing IIdPrincipalInfoRequest
public record TestRequest : APrincipalInfoRequest
{
}

//------------------------------------//

// Unit tests for CustomerOnlyValidatorTest
public class CustomerOnlyValidatorTests
{
    private readonly CustomerOnlyValidatorTest _validator;

    public CustomerOnlyValidatorTests()
    {
        _validator = new CustomerOnlyValidatorTest();
    }

    [Fact]
    public void Should_Have_Error_When_IsCustomer_Is_False()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = true,
            IsCustomer = false
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IsCustomer)
              .WithErrorMessage("Forbidden!");
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_IsCustomer_Is_True()
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
        result.ShouldNotHaveValidationErrorFor(x => x.IsCustomer);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Not_Authenticated()
    {
        // Arrange
        var request = new TestRequest
        {
            IsAuthenticated = false,
            IsCustomer = false
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsCustomer);
    }

    //------------------------------------//

}//Cls