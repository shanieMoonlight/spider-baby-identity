using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.FeatureFlags.Qry.GetById;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetById;

public class GetFeatureFlagByIdQryValidatorTests
{
    private readonly GetFeatureFlagByIdQryValidator _validator;

    public GetFeatureFlagByIdQryValidatorTests()
    {
        _validator = new();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_Id_is_null()
    {
        //Arrange
        GetFeatureFlagByIdQry cmd = new(null);

        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Id);

    }


    //------------------------------------//


    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var _validator = new GetFeatureFlagByIdQryValidator();

        // Act & Assert
        _validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetFeatureFlagByIdQry>>();
    }

    //------------------------------------//

}//Cls