using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using MyResults;

namespace ID.Infrastructure.DomainServices.Teams.Subs;
internal class TeamSubsriptionServiceFactory(IIdUnitOfWork _uow) : ITeamSubscriptionServiceFactory
{

    public async Task<GenResult<ITeamSubscriptionService>> GetServiceAsync(Guid? teamId)
    {
        var dbTeam = await _uow.TeamRepo.FirstOrDefaultAsync(new TeamByIdWithSubscriptionsSpec(teamId));
        if (dbTeam is null)
            return GenResult<ITeamSubscriptionService>.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));

        var service = new SubscriptionService(_uow, dbTeam);
        return GenResult<ITeamSubscriptionService>.Success(service);
    }

    //-----------------------//

    public Task<GenResult<ITeamSubscriptionService>> GetServiceAsync(Team? team)
    {
        if (team is null)
            return Task.FromResult(GenResult<ITeamSubscriptionService>.NotFoundResult(IDMsgs.Error.NotFound<Team>()));

        var service = new SubscriptionService(_uow, team);
        return Task.FromResult(GenResult<ITeamSubscriptionService>.Success(service));
    }


}//Cls
