using ID.Application.Utility.Attributes;

namespace ID.Application.Features.Account.Cmd.ResendEmailConfirmation;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class ResendEmailConfirmationDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

}//Cls