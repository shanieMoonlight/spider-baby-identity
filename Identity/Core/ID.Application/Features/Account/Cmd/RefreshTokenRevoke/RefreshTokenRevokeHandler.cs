using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Application.JWT;

namespace ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
public class RefreshTokenRevokeHandler(IJwtRefreshTokenService<AppUser> refreshProvider)
    : IIdCommandHandler<RefreshTokenRevokeCmd>
{

    public async Task<BasicResult> Handle(RefreshTokenRevokeCmd request, CancellationToken cancellationToken)
    {
        var user = request.PrincipalUser!;
        await refreshProvider.RevokeTokensAsync(user, cancellationToken);

        return BasicResult.Success(IDMsgs.Info.Tokens.TokensRemovedForUser(user));
    }


}//Cls

