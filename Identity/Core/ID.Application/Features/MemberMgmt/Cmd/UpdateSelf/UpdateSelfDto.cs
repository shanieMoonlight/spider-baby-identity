using ID.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
public class UpdateSelfDto
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }


    public TwoFactorProvider? TwoFactorProvider { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public Guid TeamId { get; set; }
}

