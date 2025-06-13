using ID.GlobalSettings.Constants;
using System.ComponentModel;

namespace ID.Domain.Entities.Teams;

public enum TeamType
{
    [Description("Customer Team")]
    Customer = 1,
    [Description(IdGlobalConstants.Teams.MAINTENANCE_TEAM_NAME)]
    Maintenance = 2,
    [Description(IdGlobalConstants.Teams.SUPER_TEAM_NAME)]
    Super = 3,

}//Enm