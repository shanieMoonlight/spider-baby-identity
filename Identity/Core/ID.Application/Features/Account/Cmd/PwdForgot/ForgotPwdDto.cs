using ID.Application.Utility.Attributes;

namespace ID.Application.Features.Account.Cmd.PwdForgot;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class ForgotPwdDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }


}//Cls