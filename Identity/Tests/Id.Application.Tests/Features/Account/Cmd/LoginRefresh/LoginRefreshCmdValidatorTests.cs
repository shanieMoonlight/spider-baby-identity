using ID.Application.Features.Account.Cmd.LoginRefresh;

namespace ID.Application.Tests.Features.Account.Cmd.LoginRefresh;
public class LoginRefreshCmdValidatorTests
{
    private readonly LoginRefreshCmdValidator _validator = new();


    //--------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
        LoginRefreshCmd cmd = new(null!);


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //--------------------------//

    [Fact]
    public void Should_have_error_when_RefreshToken_is_null()
    {
        //Arrange
        var dto = new LoginRefreshDto
        {
            RefreshToken = null!,
            DeviceFingerprint = null!
        };
        LoginRefreshCmd cmd = new(dto);


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.Errors.ShouldContain(e => e.PropertyName == "Dto.RefreshToken");

    }


    //--------------------------//


}//Cls
