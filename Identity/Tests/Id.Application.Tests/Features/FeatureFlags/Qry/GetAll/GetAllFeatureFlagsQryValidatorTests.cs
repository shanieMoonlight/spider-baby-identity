using ID.Application.Features.FeatureFlags.Qry.GetAll;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetAll;

public class GetAllFeatureFlagsQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetAllFeatureFlagsQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetAllFeatureFlagsQry>>();
    }

    //------------------------------------//

}//Cls