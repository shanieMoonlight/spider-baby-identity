using ID.Application.Utility.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.Login;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class LoginDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    //public string? UserIdentifier { get; set; }


    [Required]
    public string? Password { get; set; }

    public string? DeviceId { get; set; }


}//Cls