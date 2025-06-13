using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Teams.Qry.@Subs.GetSubscription;
public record GetTeamSubscriptionQry(GetTeamSubscriptionDto Dto) : AIdCommand<SubscriptionDto>;



