using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Common.Dtos.User;
public class IdentityAddressDto
{
    public string Line1 { get; init; } = string.Empty;
    public string Line2 { get; init; } = string.Empty;
    public string? Line3 { get; init; } = string.Empty;
    public string? Line4 { get; init; } = string.Empty;
    public string? Line5 { get; init; } = string.Empty;


    public string? AreaCode { get; private set; } = string.Empty;

    public string? Notes { get; private set; } = string.Empty;

    public Guid AppUserId { get; set; }

    //------------------------------------//

    #region Model Biinding CTOR
    public IdentityAddressDto() { }
    #endregion

    public IdentityAddressDto(IdentityAddress mdl)
    {
        //Id = mdl.Id;
        Line1 = mdl.Line1;
        Line2 = mdl.Line2;
        Line3 = mdl.Line3;
        Line4 = mdl.Line4;
        Line5 = mdl.Line5;
        AreaCode = mdl.AreaCode;
        Notes = mdl.Notes;
        AppUserId = mdl.AppUserId;
    }

    //------------------------------------//

}

