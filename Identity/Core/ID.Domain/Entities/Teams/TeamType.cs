using ID.GlobalSettings.Constants;
using System.ComponentModel;

namespace ID.Domain.Entities.Teams;

public enum TeamType
{
    [Description("Customer Team")]
    customer = 1,
    [Description(IdGlobalConstants.Teams.MAINTENANCE_TEAM_NAME)]
    maintenance = 2,
    [Description(IdGlobalConstants.Teams.SUPER_TEAM_NAME)]
    super = 3,

}//Enm