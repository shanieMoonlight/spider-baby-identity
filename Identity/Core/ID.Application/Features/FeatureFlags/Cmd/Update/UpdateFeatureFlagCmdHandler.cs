using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.FeatureFlags.Cmd.Update;


public class UpdateFeatureFlagCmdHandler(IIdentityFeatureFlagService _repo) : IIdCommandHandler<UpdateFeatureFlagCmd, FeatureFlagDto>
{
    public async Task<GenResult<FeatureFlagDto>> Handle(UpdateFeatureFlagCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (dto == null)
            return GenResult<FeatureFlagDto>.BadRequestResult(IDMsgs.Error.NO_DATA_SUPPLIED);

        var mdl = await _repo.GetByIdAsync(dto.Id, cancellationToken);
        if (mdl == null)
            return GenResult<FeatureFlagDto>.NotFoundResult(IDMsgs.Error.NotFound<FeatureFlag>(dto.Id));

        mdl.Update(dto);

        var entity = await _repo.UpdateAsync(mdl);

        return GenResult<FeatureFlagDto>.Success(entity!.ToDto());

    }

}//Cls
