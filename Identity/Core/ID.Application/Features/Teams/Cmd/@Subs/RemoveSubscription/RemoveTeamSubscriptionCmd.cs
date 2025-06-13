using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Teams.Cmd.Subs.RemoveSubscription;
public record RemoveTeamSubscriptionCmd(RemoveTeamSubscriptionDto Dto) : AIdCommand<TeamDto>;



