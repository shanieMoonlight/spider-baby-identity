using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using MyResults;

namespace ID.Infrastructure.DomainServices.Teams.Dvcs;
internal class TeamDeviceServiceFactory(IIdUnitOfWork _uow) : ITeamDeviceServiceFactory
{

    public async Task<GenResult<ITeamDeviceService>> GetServiceAsync(Guid? teamId, Guid? subscriptionId)
    {

        if (subscriptionId == null)
            return GenResult<ITeamDeviceService>.NotFoundResult(IDMsgs.Error.NotFound<TeamSubscription>(subscriptionId));

        var team = await _uow.TeamRepo.FirstOrDefaultAsync(new TeamByIdWithSubscriptionsSpec(teamId));
        if (team == null)
            return GenResult<ITeamDeviceService>.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));


        var tSub = team.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (tSub == null)
            return GenResult<ITeamDeviceService>.NotFoundResult(IDMsgs.Error.NotFound<TeamSubscription>(subscriptionId));

        var service = new TeamDeviceService(_uow, team, subscriptionId.Value);
        return GenResult<ITeamDeviceService>.Success(service);
    }

    //-----------------------//

    public Task<GenResult<ITeamDeviceService>> GetServiceAsync(Team team, Guid? subscriptionId)
    {
        if (subscriptionId == null)
            return Task.FromResult(GenResult<ITeamDeviceService>.NotFoundResult(IDMsgs.Error.NotFound<TeamSubscription>(subscriptionId)));

        var tSub = team.Subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
        if (tSub == null)
            return Task.FromResult(GenResult<ITeamDeviceService>.NotFoundResult(IDMsgs.Error.NotFound<TeamSubscription>(subscriptionId)));

        var service = new TeamDeviceService(_uow, team, subscriptionId.Value);
        return Task.FromResult(GenResult<ITeamDeviceService>.Success(service));
    }

}//Cls
