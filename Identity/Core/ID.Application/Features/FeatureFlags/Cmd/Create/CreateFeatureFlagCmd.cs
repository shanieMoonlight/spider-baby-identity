using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.FeatureFlags.Cmd.Create;
public record CreateFeatureFlagCmd(FeatureFlagDto Dto) : AIdCommand<FeatureFlagDto>;
