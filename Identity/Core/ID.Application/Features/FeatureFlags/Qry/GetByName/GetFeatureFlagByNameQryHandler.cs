using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Features.FeatureFlags.Qry.GetByName;
internal class GetFeatureFlagByNameQryHandler(IIdentityFeatureFlagService repo) : IIdQueryHandler<GetFeatureFlagByNameQry, FeatureFlagDto>
{

    public async Task<GenResult<FeatureFlagDto>> Handle(GetFeatureFlagByNameQry request, CancellationToken cancellationToken)
    {
        var name = request.Name;
        var mdl = await repo.GetByNameAsync(name, cancellationToken);
        if (mdl == null)
            return GenResult<FeatureFlagDto>.NotFoundResult(IDMsgs.Error.NotFound<FeatureFlag>(name));

        return GenResult<FeatureFlagDto>.Success(mdl.ToDto());

    }


}//Cls
