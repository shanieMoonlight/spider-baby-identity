using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Application.JWT;
using ID.Application.Mediatr.CqrsAbs;
using ID.Application.Utility.ExtensionMethods;
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

        var signInResult = await _preSignInService.Authenticate(dto, cancellationToken);

        if (signInResult.TwoFactorRequired)
        {
            var pkg = await _jwtPackageProvider.CreateJwtPackageWithTwoFactorRequiredAsync(
                 signInResult.User!, 
                 signInResult.MfaResultData?.TwoFactorProvider ?? TwoFactorProvider.Email,
                 signInResult.MfaResultData?.ExtraInfo,
                 cancellationToken);
            return GenResult<JwtPackage>.PreconditionRequiredResult(pkg);
        }

        if (signInResult.Succeeded)
        {
            var pkg = await _jwtPackageProvider.CreateJwtPackageAsync(
                signInResult.User!,
                signInResult.Team!,
                false,
                dto.DeviceId,
                cancellationToken);
            return GenResult<JwtPackage>.Success(pkg);
        }


        return signInResult.ToGenResultFailure<JwtPackage>();
    }

}//Cls

