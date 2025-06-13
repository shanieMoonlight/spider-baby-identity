using ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.RefreshRevoke;
public class RefreshTokenRevokeCmdValidatorTests
{
    private readonly RefreshTokenRevokeCmdValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<RefreshTokenRevokeCmd>>(_validator, exactMatch: false);

        // Act & Assert
        _validator.ShouldBeAssignableTo<IsAuthenticatedValidator<RefreshTokenRevokeCmd>>();
    }

    //------------------------------------//

}//Cls
