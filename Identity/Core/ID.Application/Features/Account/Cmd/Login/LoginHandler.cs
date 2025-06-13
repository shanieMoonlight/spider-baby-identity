using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.GlobalSettings.Setup.Options;
using Microsoft.Extensions.Options;
using MyResults;

namespace ID.Application.Features.Account.Cmd.Login;

/// <summary>
/// Handles user authentication requests and generates JWT token packages for successful logins.
/// Performs comprehensive security validation including credential verification, account status checks,
/// and multi-factor authentication enforcement before issuing tokens.
/// </summary>
/// <param name="_preSignInService">Service responsible for pre-authentication validation and user loading</param>
/// <param name="_jwtPackageProvider">Service that generates and configures JWT access tokens with appropriate claims</param>
public class LoginHandler(
    IPreSignInService<AppUser> _preSignInService,
    IJwtPackageProvider _jwtPackageProvider)
    : IIdCommandHandler<LoginCmd, JwtPackage>
{


    /// <summary>
    /// Handles the login command.
    /// </summary>
    /// <param name="request">The login command request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing the JWT package.</returns>
    public async Task<GenResult<JwtPackage>> Handle(LoginCmd request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Check if user exists
        var signInResult = await _preSignInService.Authenticate(dto, cancellationToken);

        if (signInResult.NotFound)
            return GenResult<JwtPackage>.NotFoundResult(signInResult.Message);

        if (signInResult.Unauthorized)
            return GenResult<JwtPackage>.UnauthorizedResult(signInResult.Message);

        if (signInResult.EmailConfirmedRequired)
            return GenResult<JwtPackage>.PreconditionRequiredResult(signInResult.Message);

        if (signInResult.TwoFactorRequired)
            return await SendTwoFactor(signInResult.Team!, signInResult.User!, signInResult.MfaResultData, dto.DeviceId);

        if (!signInResult.Succeeded)
            return signInResult.ToGenResultFailure<JwtPackage>();

        var user = signInResult.User!;
        var team = signInResult.Team!;


        JwtPackage jwtPackage = await _jwtPackageProvider.CreateJwtPackageAsync(
            user,
            team,
            false,
            dto.DeviceId,
            cancellationToken);

        return GenResult<JwtPackage>.Success(jwtPackage);
    }

    //-----------------------------//

    /// <summary>
    /// Sends a two-factor authentication request.
    /// </summary>
    /// <param name="team">The team of the user.</param>
    /// <param name="user">The user.</param>
    /// <param name="mfaResultData">The MFA result data.</param>
    /// <param name="deviceId">The device ID.</param>
    /// <returns>Result containing the JWT package with two-factor required.</returns>
    private async Task<GenResult<JwtPackage>> SendTwoFactor(Team team, AppUser user, MfaResultData? mfaResultData, string? deviceId)
    {
        var pkg = await _jwtPackageProvider.CreateJwtPackageWithTwoFactorRequiredAsync(
            user,
            team!,
            mfaResultData?.TwoFactorProvider ?? TwoFactorProvider.Email,
            mfaResultData?.ExtraInfo,
            deviceId);
        return GenResult<JwtPackage>.Success(pkg);
    }


}//Cls

