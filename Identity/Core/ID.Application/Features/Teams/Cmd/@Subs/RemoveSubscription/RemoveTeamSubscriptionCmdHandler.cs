using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams.Subs;

namespace ID.Application.Features.Teams.Cmd.Subs.RemoveSubscription;
public class RemoveTeamSubscriptionCmdHandler(ITeamSubscriptionServiceFactory subsServiceFactory)
     : IIdCommandHandler<RemoveTeamSubscriptionCmd, TeamDto>
{
    public async Task<GenResult<TeamDto>> Handle(RemoveTeamSubscriptionCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var getServiceResult = await subsServiceFactory.GetServiceAsync(dto.TeamId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<TeamDto>();

        ITeamSubscriptionService service = getServiceResult.Value!;

        var removeResult = await service.RemoveSubscriptionAsync(dto.SubscriptionId);
        return removeResult.Convert(s => service.Team.ToDto());
    }
}//Cls

