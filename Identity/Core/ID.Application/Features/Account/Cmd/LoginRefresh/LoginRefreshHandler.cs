using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;
using MyResults;

namespace ID.Application.Features.Account.Cmd.LoginRefresh;
public class RefreshTknHandler(
    IJwtRefreshTokenService<AppUser> _tknService,
    IJwtPackageProvider _jwtPackageProvider,
    IOptions<IdGlobalOptions> _globalOptionsProvider)
    : IIdCommandHandler<LoginRefreshCmd, JwtPackage>
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;


    //-----------------------------//


    public async Task<GenResult<JwtPackage>> Handle(LoginRefreshCmd request, CancellationToken cancellationToken)
    {
        if (!_globalOptions.JwtRefreshTokensEnabled)
            return GenResult<JwtPackage>.BadRequestResult(IDMsgs.Error.REFRESH_TOKEN_DISABLED);

        var tknPayload = request.RefreshToken;
        var refreshToken = await _tknService.FindTokenWithUserAndTeamAsync(tknPayload, cancellationToken);

        if (refreshToken == null)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);

        if (refreshToken.IsExpired)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);

        var user = refreshToken.User;
        if (user == null)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH_EXPIRED_TOKEN);

        var team = user.Team;
        if (team == null)
            return GenResult<JwtPackage>.NotFoundResult(IDMsgs.Error.NotFound<Team>(refreshToken));


        JwtPackage jwtPackage = await _jwtPackageProvider.RefreshJwtPackageAsync(
            existingToken: refreshToken,
            user: user!,
            team: team!,
            currentDeviceId: request.DeviceId);


        return GenResult<JwtPackage>.Success(jwtPackage);
    }

}//Cls
