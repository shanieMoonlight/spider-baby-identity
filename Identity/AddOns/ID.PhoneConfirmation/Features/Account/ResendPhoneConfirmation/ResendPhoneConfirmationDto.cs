using ID.Application.Utility.Attributes;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class ResendPhoneConfirmationDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

}//Cls