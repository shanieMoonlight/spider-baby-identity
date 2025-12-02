using ID.Domain.Repos;

namespace ID.Application.Features.IdRefreshTokens.Qry.GetAll;
internal class GetAllIdRefreshTokensQryHandler(IIdentityRefreshTokenRepo repo) : IIdQueryHandler<GetAllIdRefreshTokensQry, IEnumerable<IdRefreshTokenDto>>
{

    public async Task<GenResult<IEnumerable<IdRefreshTokenDto>>> Handle(GetAllIdRefreshTokensQry request, CancellationToken cancellationToken)
    {
        var mdls = await repo.ListAllAsync();
        var dtos = mdls.Select(mdl => mdl.ToDto());
        return GenResult<IEnumerable<IdRefreshTokenDto>>.Success(dtos);

    }

}//Cls
