using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using MyResults;

namespace ID.Application.Features.FeatureFlags.Qry.GetAllByName;
internal class GetAllFeatureFlagsByNameQryHandler(IIdentityFeatureFlagService flagService) 
    : IIdQueryHandler<GetAllFeatureFlagsByNameQry, IEnumerable<FeatureFlagDto>>
{

    public async Task<GenResult<IEnumerable<FeatureFlagDto>>> Handle(GetAllFeatureFlagsByNameQry request, CancellationToken cancellationToken)
    {
        var name = request.Name;
        var mdls = await flagService.GetAllByNameAsync(name, cancellationToken);

        var dtos = mdls.Select(m => m.ToDto());

        return GenResult< IEnumerable<FeatureFlagDto>>.Success(dtos);

    }


}//Cls
