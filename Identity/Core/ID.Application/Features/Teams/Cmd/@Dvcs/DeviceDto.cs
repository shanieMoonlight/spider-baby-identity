using ID.Domain.Entities.Teams;

namespace ID.Application.Features.Teams.Cmd.Dvcs;
public class DeviceDto
{
    public Guid? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string UniqueId { get; set; } = string.Empty;

    public Guid SubscriptionId { get; set; }

    //------------------------------------//

    #region ModelBinding CTOR
    public DeviceDto() { }
    #endregion

    //- - - - - - - - - - - - - - - - - - //

    public DeviceDto(TeamDevice mdl)
    {
        Id = mdl.Id;
        Name = mdl.Name;
        Description = mdl.Description;
        UniqueId = mdl.UniqueId;
        SubscriptionId = mdl.SubscriptionId;
    }

    //------------------------------------//

}//Cls

