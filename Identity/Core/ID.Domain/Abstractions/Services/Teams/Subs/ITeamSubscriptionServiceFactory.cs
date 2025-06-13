using ID.Domain.Entities.Teams;
using MyResults;

namespace ID.Domain.Abstractions.Services.Teams.Subs;
public interface ITeamSubscriptionServiceFactory
{
    Task<GenResult<ITeamSubscriptionService>> GetServiceAsync(Guid? teamId);
    Task<GenResult<ITeamSubscriptionService>> GetServiceAsync(Team? team);
}