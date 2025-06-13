using ID.Domain.Entities.Teams;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.Account.Cmd.AddMntcMember;
public class AddMntcMemberDto
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public int TeamPosition { get; set; } = 1;



}

