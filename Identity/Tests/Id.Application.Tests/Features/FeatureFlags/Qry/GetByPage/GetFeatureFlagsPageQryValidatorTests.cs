using ID.Application.Features.FeatureFlags.Qry.GetPage;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetByPage;

public class GetFeatureFlagsPageQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetFeatureFlagsPageQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetFeatureFlagsPageQry>>();
    }

    //------------------------------------//

}//Cls