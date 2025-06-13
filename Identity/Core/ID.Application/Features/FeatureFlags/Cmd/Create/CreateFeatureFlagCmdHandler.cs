using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.SubPlans;
using MyResults;


namespace ID.Application.Features.FeatureFlags.Cmd.Create;

public class CreateFeatureFlagCmdHandler(IIdentityFeatureFlagService _repo) : IIdCommandHandler<CreateFeatureFlagCmd, FeatureFlagDto>
{

    public async Task<GenResult<FeatureFlagDto>> Handle(CreateFeatureFlagCmd request, CancellationToken cancellationToken)
    {

        var mdl = request.Dto.ToModel();

        var dbMdl = await _repo.AddAsync(mdl, cancellationToken);

        return GenResult<FeatureFlagDto>.Success(dbMdl.ToDto());

    }


}//Cls
