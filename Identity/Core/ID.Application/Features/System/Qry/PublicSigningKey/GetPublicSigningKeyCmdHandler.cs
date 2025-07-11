using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using MyResults;
using ID.Application.JWT;

namespace ID.Application.Features.System.Qry.PublicSigningKey;
public class GetPublicSigningKeyCmdHandler(IJwtCurrentKeyService _keyService) : IIdCommandHandler<GetPublicSigningKeyCmd, PublicSigningKeyDto>
{

    public async Task<GenResult<PublicSigningKeyDto>> Handle(GetPublicSigningKeyCmd request, CancellationToken cancellationToken)
    {
        var key = await _keyService.GetPublicSigningKeyAsync();
        return key == null
            ? GenResult<PublicSigningKeyDto>.NotFoundResult(IDMsgs.Error.Jwt.SYMETRIC_CRYPTO_HAS_NO_PUBLIC_KEY)
            : GenResult<PublicSigningKeyDto>.Success(new PublicSigningKeyDto(key));
    }

}//Cls

