using ID.Application.Features.Mntc.Qry.EmailRoutes;
using ID.Application.Features.Mntc.Qry.PublicSigningKey;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Mntc.PublicSigningKey;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class GetPublicSigningKeyCmdValidatorTests
{

    [Fact]
    public void DowsNotRequireAuthentication()
    {
        // Arrange
        var validator = new GetPublicSigningKeyCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<GetPublicSigningKeyCmd>>();
    }

    //------------------------------------//

}//Cls
