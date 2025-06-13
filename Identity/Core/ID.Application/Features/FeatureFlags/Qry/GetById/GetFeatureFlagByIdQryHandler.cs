using MyResults;
using ID.Application.Features.FeatureFlags;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Features.FeatureFlags.Qry.GetById;
internal class GetFeatureFlagByIdQryHandler(IIdentityFeatureFlagService repo) : IIdQueryHandler<GetFeatureFlagByIdQry, FeatureFlagDto>
{
    public async Task<GenResult<FeatureFlagDto>> Handle(GetFeatureFlagByIdQry request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var mdl = await repo.GetByIdAsync(id, cancellationToken);
        if (mdl is null)
            return GenResult<FeatureFlagDto>.NotFoundResult(IDMsgs.Error.NotFound<FeatureFlag>(id));

        return GenResult<FeatureFlagDto>.Success(mdl.ToDto());
    }

}//Cls
