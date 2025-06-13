using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.FeatureFlags.Cmd.Delete;
public record DeleteFeatureFlagCmd(Guid Id) : AIdCommand;
