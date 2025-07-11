using ID.Application.JWT;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.System.Qry.JWKS;
public record GetJwksCmd() : AIdCommand<JwkListDto>;



