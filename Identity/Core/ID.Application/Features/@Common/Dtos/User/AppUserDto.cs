using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;
using System.Text.Json.Serialization;

namespace ID.Application.Features.Common.Dtos.User;
public class AppUserDto : AuditableEntityDto
{
    public Guid? Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; } = string.Empty;

    public Guid TeamId { get; set; }

    public int TeamPosition { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TwoFactorProvider TwoFactorProvider { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool EmailConfirmed { get; set; }

    public IdentityAddressDto? Address { get; set; }

    //------------------------------------//

    #region ModelBindingCtor
    public AppUserDto() { }
    #endregion

    public AppUserDto(AppUser mdl) : base(mdl)
    {
        Id = mdl.Id;
        FirstName = mdl.FirstName ?? string.Empty;
        LastName = mdl.LastName ?? string.Empty;
        PhoneNumber = mdl.PhoneNumber;
        UserName = mdl.UserName;
        Email = mdl.Email ?? string.Empty;
        TeamId = mdl.TeamId;
        TeamPosition = mdl.TeamPosition;
        TwoFactorProvider = mdl.TwoFactorProvider;
        TwoFactorEnabled = mdl.TwoFactorEnabled;
        EmailConfirmed = mdl.EmailConfirmed;
        Address = mdl.Address.ToDto();
    }

    //------------------------------------//

}

