using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;
public record RemoveFeaturesFromSubscriptionPlanCmd(RemoveFeaturesFromSubscriptionPlanDto Dto) : AIdCommand<SubscriptionPlanDto>;



