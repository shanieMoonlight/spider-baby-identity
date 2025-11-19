using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.OAuth.Amazon.Services.Abs;
using MyResults;

namespace ID.OAuth.Amazon.Features.SignIn.AmazonSignIn;

public class AmazonSignInHandler(
    IFindOrCreateService<AppUser> _findOrCreate,
    IJwtPackageProvider _jwtPackageProvider,
    IAmazonAuthenticationService _verifier,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ITwoFactorMsgService _twoFactorMsgService)
    : IIdCommandHandler<AmazonSignInCmd, JwtPackage>
{
    public async Task<GenResult<JwtPackage>> Handle(AmazonSignInCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var verifyResult = await _verifier.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, cancellationToken);
        if (!verifyResult.Succeeded)
            return verifyResult.Convert<JwtPackage>();

        var userProfile = verifyResult.Value!;

        var userResult = await _findOrCreate.FindOrCreateUserAsync(userProfile, dto, cancellationToken);
        if (!userResult.Succeeded)
            return userResult.Convert<JwtPackage>();

        var user = userResult.Value!;
        var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);

        return tfEnabled
            ? await SendTwoFactorAndReturnJwtPackageAsync(user: user, team: user.Team!, cancellationToken: cancellationToken)
            : await ReturnStandardJwtPackageAsync(user: user, team: user.Team!, currentDeviceId: dto.DeviceId, cancellationToken: cancellationToken);
    }

    //---------------------//

    private async Task<GenResult<JwtPackage>> ReturnStandardJwtPackageAsync(AppUser user, Team team, string? currentDeviceId, CancellationToken cancellationToken)
    {
        var jwt = await _jwtPackageProvider.CreateJwtPackageAsync(user: user, team: team, currentDeviceId: currentDeviceId, cancellationToken: cancellationToken);
        return GenResult<JwtPackage>.Success(jwt);
    }

    //---------------------//

    private async Task<GenResult<JwtPackage>> SendTwoFactorAndReturnJwtPackageAsync(AppUser user, Team team, CancellationToken cancellationToken)
    {
        var tfResult = await _twoFactorMsgService.SendOTPFor2FactorAuth(team, user);
        if (!tfResult.Succeeded)
            return GenResult<JwtPackage>.Failure(tfResult.Info);

        var mfa = tfResult.Value!;
        var jwt = await _jwtPackageProvider.CreateJwtPackageWithTwoFactorRequiredAsync(user: user, provider: mfa.TwoFactorProvider, extraInfo: mfa.ExtraInfo, cancellationToken: cancellationToken);
        return GenResult<JwtPackage>.Success(jwt);
    }
}
