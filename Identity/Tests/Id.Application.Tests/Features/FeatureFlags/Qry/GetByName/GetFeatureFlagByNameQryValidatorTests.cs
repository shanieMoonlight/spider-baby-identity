using ID.Application.Features.FeatureFlags.Qry.GetAllByName;
using ID.Application.Features.FeatureFlags.Qry.GetByName;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetByName;

public class GetFeatureFlagByNameQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetFeatureFlagByNameQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetFeatureFlagByNameQry>>();
    }

    //------------------------------------//

}//Cls