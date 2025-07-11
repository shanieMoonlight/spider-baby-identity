using ID.Application.Features.System.Qry.PublicSigningKey;
using ID.Application.Mediatr.Validation;

namespace ID.Application.Tests.Features.System.PublicSigningKey;

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

}//Cls
