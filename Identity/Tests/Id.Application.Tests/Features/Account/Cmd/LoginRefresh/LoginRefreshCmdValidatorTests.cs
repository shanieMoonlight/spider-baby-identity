using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.LoginRefresh;
using ID.Application.Features.Account.Cmd.Refresh;

namespace ID.Application.Tests.Features.Account.Cmd.LoginRefresh;
public class LoginRefreshCmdValidatorTests
{
    private readonly LoginRefreshCmdValidator _validator = new();


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        LoginRefreshCmd cmd = new(null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.RefreshToken);

    }


    //------------------------------------//


}//Cls
