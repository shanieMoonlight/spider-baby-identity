using ID.Domain.Entities.Teams;
using MyResults;

namespace ID.Domain.Abstractions.Services.Teams.Dvcs;
public interface ITeamDeviceServiceFactory
{
    Task<GenResult<ITeamDeviceService>> GetServiceAsync(Guid? teamId, Guid? subscriptionId);
    Task<GenResult<ITeamDeviceService>> GetServiceAsync(Team team, Guid? subscriptionId);
}