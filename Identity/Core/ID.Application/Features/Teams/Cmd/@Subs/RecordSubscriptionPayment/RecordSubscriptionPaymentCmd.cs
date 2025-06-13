using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Teams.Cmd.Subs.RecordSubscriptionPayment;
public record RecordSubscriptionPaymentCmd(RecordSubscriptionPaymentDto Dto) : AIdCommand<SubscriptionDto>;



