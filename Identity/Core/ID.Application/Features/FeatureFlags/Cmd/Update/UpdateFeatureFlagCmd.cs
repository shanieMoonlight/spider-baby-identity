using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.FeatureFlags.Cmd.Update;
public record UpdateFeatureFlagCmd(FeatureFlagDto Dto) : AIdCommand<FeatureFlagDto>;
