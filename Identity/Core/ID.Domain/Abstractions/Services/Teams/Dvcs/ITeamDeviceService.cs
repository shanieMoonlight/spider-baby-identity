using ClArch.ValueObjects;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Teams;
using MyResults;

namespace ID.Domain.Abstractions.Services.Teams.Dvcs;
public interface ITeamDeviceService
{
    TeamSubscription Subscription { get; }
    Team Team { get; }

    Task<GenResult<TeamDevice>> AddDeviceAsync(Name name, DescriptionNullable description, UniqueId uniqueId);
    Task<TeamDevice?> GetDeviceAsync(Guid? dvcId);
    Task<GenResult<bool>> RemoveDeviceAsync(Guid? dvcId);
    Task<bool> RemoveDeviceAsync(TeamDevice sub);
    Task<Team> UpdateAndSaveChangesAsync();

    /// <summary>
    /// Updates the device with the given id
    /// Can only upate Name and Description. If anything else needs to change Remove the device and add a new one
    /// </summary>
    /// <param name="dvcId">Device Identifier</param>
    /// <param name="name">New name</param>
    /// <param name="description">New description</param>
    /// <returns>Result with update device</returns>
    Task<GenResult<TeamDevice>> UpdateDeviceAsync(Guid? dvcId, Name name, DescriptionNullable description);
}