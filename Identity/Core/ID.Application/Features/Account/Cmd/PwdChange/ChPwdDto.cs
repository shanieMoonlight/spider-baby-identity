using ID.Application.Utility.Attributes;
using ID.Domain.Utility.Messages;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.PwdChange;

[AtLeastOneProperty(nameof(Username), nameof(UserId), nameof(Email))]
public class ChPwdDto
{
    public Guid? UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = IDMsgs.Error.Authorization.NON_MATCHING_PASSOWRDS)]
    public string? ConfirmPassword { get; set; } = string.Empty;

}//Cls