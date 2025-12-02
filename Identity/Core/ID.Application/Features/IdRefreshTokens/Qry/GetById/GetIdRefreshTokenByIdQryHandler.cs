using ID.Domain.Entities.Refreshing;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.RefreshTokens;

namespace ID.Application.Features.IdRefreshTokens.Qry.GetById;
internal class GetIdRefreshTokenByIdQryHandler(IIdentityRefreshTokenRepo _repo) : IIdQueryHandler<GetIdRefreshTokenByIdQry, IdRefreshTokenDto>
{

    public async Task<GenResult<IdRefreshTokenDto>> Handle(GetIdRefreshTokenByIdQry request, CancellationToken cancellationToken)
    {
        var id = request.Id;

        var spec = RefreshTokenByIdWithUserAndDeviceAndTeamSpec.Create(id);
        var mdl = await _repo.FirstOrDefaultAsync(spec, cancellationToken);
        if (mdl is null)
            return GenResult<IdRefreshTokenDto>.NotFoundResult(IDMsgs.Error.NotFound<IdRefreshToken>(id));

        return GenResult<IdRefreshTokenDto>.Success(mdl.ToDto());

    }


}//Cls
