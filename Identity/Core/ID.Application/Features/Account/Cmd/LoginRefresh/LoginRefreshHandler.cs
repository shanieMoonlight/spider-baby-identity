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
public class LoginRefreshHandler(
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

        var requestDto = request.Dto;
        var tknPayload = requestDto.RefreshToken;
        var dvcFingerprint = requestDto.DeviceFingerprint;
        var refreshToken = await _tknService.FindTokenWithUserAndDeviceAndTeamAsync(tknPayload, cancellationToken);


        if (refreshToken == null)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        if (refreshToken.IsExpired)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        var user = refreshToken.User;
        if (user == null)
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH_EXPIRED_TOKEN);


        var team = user.Team;
        if (team == null)
            return GenResult<JwtPackage>.NotFoundResult(IDMsgs.Error.NotFound<Team>(user.TeamId));


        if (!DoFingerprintsMatch(refreshToken.TrustedDevice?.Fingerprint, dvcFingerprint))
            return GenResult<JwtPackage>.UnauthorizedResult(IDMsgs.Error.Authorization.INVALID_AUTH);


        JwtPackage jwtPackage = await _jwtPackageProvider.RefreshJwtPackageAsync(
            existingToken: refreshToken,
            currentClientToken: tknPayload,
            user: user!,
            team: team!,
            currentDeviceFingerprint: dvcFingerprint);


        return GenResult<JwtPackage>.Success(jwtPackage);
    }

    //-----------------------------//

    private static bool DoFingerprintsMatch(string? tokenFingerprint, string? requestFingerprint)
    {
        var tokenFingerprintTrimmed = tokenFingerprint?.Trim();
        var requestFingerprintTrimmed = requestFingerprint?.Trim();

        // If the token is not associated with a device, don't enforce fingerprint matching.
        if (string.IsNullOrWhiteSpace(tokenFingerprintTrimmed))
            return true;

        // Token has a fingerprint, but request does not -> fail.
        if (string.IsNullOrWhiteSpace(requestFingerprintTrimmed))
            return false;

        return tokenFingerprintTrimmed == requestFingerprintTrimmed;
    }


}//Cls
