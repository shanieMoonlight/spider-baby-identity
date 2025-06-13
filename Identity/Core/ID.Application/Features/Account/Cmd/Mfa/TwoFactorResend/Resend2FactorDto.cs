using ID.Application.Utility.Attributes;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class Resend2FactorDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

}//Cls