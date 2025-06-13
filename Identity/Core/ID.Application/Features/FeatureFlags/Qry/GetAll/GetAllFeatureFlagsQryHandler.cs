using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using MyResults;

namespace ID.Application.Features.FeatureFlags.Qry.GetAll;


internal class GetAllFeatureFlagsQryHandler(IIdentityFeatureFlagService repo) : IIdQueryHandler<GetAllFeatureFlagsQry, IEnumerable<FeatureFlagDto>>
{

    public async Task<GenResult<IEnumerable<FeatureFlagDto>>> Handle(GetAllFeatureFlagsQry request, CancellationToken cancellationToken)
    {
        var mdls = await repo.GetAllAsync();
        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<FeatureFlagDto>>.Success(dtos);

    }

}//Cls
