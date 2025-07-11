using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using MyResults;

namespace ID.Application.Features.System.Qry.JWKS;
public class GetJwksCmdHandler(
    IJsonWebKeyProvider _keyProvider

    )
    : IIdCommandHandler<GetJwksCmd, JwkListDto>
{
    public async Task<GenResult<JwkListDto>> Handle(GetJwksCmd request, CancellationToken cancellationToken)
    {
        var dto = await _keyProvider.GetJwks();
        return GenResult<JwkListDto>.Success(dto);
    }
}

