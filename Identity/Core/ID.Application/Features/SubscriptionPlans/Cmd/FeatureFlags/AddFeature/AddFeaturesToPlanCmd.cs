using ID.Application.Features.SubscriptionPlans;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;
public record AddFeatureToSubscriptionPlanCmd(AddFeaturesToPlanDto Dto) : AIdCommand<SubscriptionPlanDto>;



