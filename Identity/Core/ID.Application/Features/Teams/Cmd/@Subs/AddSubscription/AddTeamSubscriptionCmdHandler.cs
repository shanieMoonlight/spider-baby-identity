using MyResults;
using ID.Application.Features.Teams;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Abstractions.Services.Teams.Subs;

namespace ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
public class AddTeamSubscriptionCmdHandler(ITeamSubscriptionServiceFactory subsServiceFactory)
    : IIdCommandHandler<AddTeamSubscriptionCmd, TeamDto>
{

    public async Task<GenResult<TeamDto>> Handle(AddTeamSubscriptionCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var getServiceResult = await subsServiceFactory.GetServiceAsync(dto.TeamId);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<TeamDto>();

        ITeamSubscriptionService service = getServiceResult.Value!;

        var addResult = await service.AddSubscriptionAsync(dto.SubscriptionPlanId, Discount.Create(dto.Discount));

        return addResult.Convert(s => service.Team.ToDto());
    }

}//Cls

