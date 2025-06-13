using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
public record AddTeamSubscriptionCmd(AddTeamSubscriptionDto Dto) : AIdCommand<TeamDto>;



