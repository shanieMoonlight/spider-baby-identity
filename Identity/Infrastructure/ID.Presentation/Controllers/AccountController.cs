using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Authenticators.Teams;
using ID.Application.Dtos.Account.Cookies;
using ID.Application.Features.Account.Cmd.AddMntcMember;
using ID.Application.Features.Account.Cmd.AddSprMember;
using ID.Application.Features.Account.Cmd.ConfirmEmail;
using ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;
using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Application.Features.Account.Cmd.Cookies.SignOut;
using ID.Application.Features.Account.Cmd.Cookies.TwoFactorResendCookie;
using ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;
using ID.Application.Features.Account.Cmd.Login;
using ID.Application.Features.Account.Cmd.LoginRefresh;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorDisable;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorEnable;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
using ID.Application.Features.Account.Cmd.PwdChange;
using ID.Application.Features.Account.Cmd.PwdForgot;
using ID.Application.Features.Account.Cmd.PwdReset;
using ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmation;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmationPrincipal;
using ID.Application.Features.Account.Qry.GetTwoFactorAppSetupData;
using ID.Application.Features.Account.Qry.MyInfo;
using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Models;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender) : ControllerBase
{

    /// <summary>
    /// Confirms a user's email address using a confirmation token sent to their email.
    /// </summary>
    /// <param name="dto">The confirmation data including email and token.</param>
    /// <returns>A message indicating success or failure of email confirmation.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmEmail([FromBody] ConfirmEmailDto dto) =>
        this.ProcessResult(await sender.Send(new ConfirmEmailCmd(dto)));

    //------------------------//

    /// <summary>
    /// Confirms a user's email address and sets their password in a single step.
    /// </summary>
    /// <param name="dto">The confirmation data including email, token, and new password.</param>
    /// <returns>A message indicating success or failure of the operation.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmEmailWithPassword([FromBody] ConfirmEmailWithPwdDto dto) =>
    this.ProcessResult(await sender.Send(new ConfirmEmailWithPwdCmd(dto)));

    //------------------------//

    /// <summary>
    /// Initiates the forgot password process by sending a reset link to the user's email.
    /// </summary>
    /// <param name="dto">The forgot password data including email or username.</param>
    /// <returns>A message indicating whether the reset email was sent.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ForgotPassword([FromBody] ForgotPwdDto dto) =>
        this.ProcessResult(await sender.Send(new ForgotPwdCmd(dto)));

    //------------------------//

    /// <summary>
    /// Authenticates a user and returns a JWT package if successful.
    /// </summary>
    /// <param name="dto">The login data including username/email and password.</param>
    /// <returns>A JWT package containing access and refresh tokens, or two-factor requirements.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<JwtPackage>> Login([FromBody] LoginDto dto) =>
           this.ProcessResult(await sender.Send(new LoginCmd(dto)));

    //------------------------//

    /// <summary>
    /// Refreshes the JWT access token using a valid refresh token and device ID.
    /// </summary>
    /// <param name="dto">The refresh token data including reset token and device ID.</param>
    /// <returns>A new JWT package or a message if the refresh fails.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> LoginRefresh([FromBody]  LoginRefreshDto dto ) =>
           this.ProcessResult(await sender.Send(new LoginRefreshCmd(dto.ResetToken, dto.DeviceId)));

    //------------------------//

    /// <summary>
    /// Signs out the current user from cookie authentication.
    /// </summary>
    /// <returns>A message indicating the result of the sign-out operation.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]    
    public async Task<IActionResult> CookieSignOut()
    {
        var result = await sender.Send(new SignOutCmd());
        if (!result.Succeeded)
            return this.ProcessFailedResult(result);

        return Ok(MessageResponseDto.Generate("Signed Out"));
    }

    //------------------------//

    /// <summary>
    /// Signs in a user using cookies. Supports two-factor and email confirmation requirements.
    /// </summary>
    /// <param name="dto">The sign-in data including credentials and RememberMe flag.</param>
    /// <returns>Sign-in result data, including two-factor or email confirmation requirements if needed.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<CookieSignInResultData>> CookieSignIn([FromBody] CookieSignInDto dto) =>
           this.ProcessResult(await sender.Send(new CookieSignInCmd(dto)));

    //------------------------//

    /// <summary>
    /// Changes the password for a user. Requires authentication.
    /// </summary>
    /// <param name="dto">The change password data including current and new passwords.</param>
    /// <returns>A message indicating the result of the password change.</returns>
    [HttpPost("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ChangePassword([FromBody] ChPwdDto dto) =>
           this.ProcessResult(await sender.Send(new ChPwdCmd(dto)));

    //------------------------//

    /// <summary>
    /// Retrieves the current authenticated user's profile information.
    /// </summary>
    /// <returns>The current user's profile as an AppUserDto.</returns>
    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> MyInfo() =>
        this.ProcessResult(await sender.Send(new MyInfoQry()));

    //------------------------//

    /// <summary>
    /// Adds a new member to the Maintenance team. Requires Maintenance-level authorization.
    /// </summary>
    /// <param name="dto">The new member's details.</param>
    /// <returns>The created user's profile.</returns>
    [HttpPost("[action]")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> AddMntcTeamMember([FromBody] AddMntcMemberDto dto) =>
           this.ProcessResult(await sender.Send(new AddMntcMemberCmd(dto)));

    //------------------------//

    /// <summary>
    /// Adds a new member to the Super team. Requires Super-level authorization.
    /// </summary>
    /// <param name="dto">The new member's details.</param>
    /// <returns>The created user's profile.</returns>
    [HttpPost("[action]")]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> AddSuperTeamMember([FromBody] AddSprMemberDto dto) =>
        this.ProcessResult(await sender.Send(new AddSprMemberCmd(dto)));

    //------------------------//

    /// <summary>
    /// Revokes the current user's refresh token, invalidating all sessions.
    /// </summary>
    /// <returns>A message indicating the result of the revocation.</returns>
    [HttpPost("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> RefreshTokenRevoke() =>
           this.ProcessResult(await sender.Send(new RefreshTokenRevokeCmd()));

    //------------------------//

    /// <summary>
    /// Resends the email confirmation link to the specified email address.
    /// </summary>
    /// <param name="dto">The email confirmation resend data.</param>
    /// <returns>A message indicating whether the email was sent.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendEmailConfirmation([FromBody] ResendEmailConfirmationDto dto) =>
           this.ProcessResult(await sender.Send(new ResendEmailConfirmationCmd(dto)));

    //------------------------//

    /// <summary>
    /// Resends the email confirmation link to the currently authenticated user.
    /// </summary>
    /// <returns>A message indicating whether the email was sent.</returns>
    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ResendEmailConfirmationAuthorized() =>
        this.ProcessResult(await sender.Send(new ResendEmailConfirmationPrincipalCmd()));

    //------------------------//

    /// <summary>
    /// Resends the email confirmation link to the specified email address (anonymous access).
    /// </summary>
    /// <param name="email">The email address to resend confirmation to.</param>
    /// <returns>A message indicating whether the email was sent.</returns>
    [HttpGet("[action]/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendEmailConfirmation(string email) =>
        this.ProcessResult(await sender.Send(new ResendEmailConfirmationCmd(new() { Email = email })));

    //------------------------//

    /// <summary>
    /// Resends a two-factor authentication code to the user.
    /// </summary>
    /// <param name="dto">The two-factor resend data.</param>
    /// <returns>A message indicating whether the code was sent.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendTwoFactor([FromBody] Resend2FactorDto dto) =>
           this.ProcessResult(await sender.Send(new Resend2FactorCmd(dto)));

    //------------------------//

    /// <summary>
    /// Resends a two-factor authentication code for cookie-based authentication.
    /// </summary>
    /// <returns>A message indicating whether the code was sent.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendTwoFactorCookie() =>
           this.ProcessResult(await sender.Send(new Resend2FactorCookieCmd()));

    //------------------------//

    /// <summary>
    /// Verifies a two-factor authentication code for the user.
    /// </summary>
    /// <param name="dto">The verification data including code and provider.</param>
    /// <returns>A JWT package if verification is successful.</returns>
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult<JwtPackage>> TwoFactorVerification([FromBody] Verify2FactorDto dto) =>
        this.ProcessResult(await sender.Send(new Verify2FactorCmd(dto)));

    //------------------------//

    /// <summary>
    /// Verifies a two-factor authentication code for cookie-based authentication.
    /// </summary>
    /// <param name="dto">The verification data including code and provider.</param>
    /// <returns>A JWT package if verification is successful.</returns>
    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<ActionResult<JwtPackage>> TwoFactorVerificationCookie([FromBody] Verify2FactorCookieDto dto) =>
        this.ProcessResult(await sender.Send(new Verify2FactorCookieCmd(dto)));

    //------------------------//

    /// <summary>
    /// Retrieves setup data for configuring an authenticator app for two-factor authentication.
    /// </summary>
    /// <returns>Setup data including QR code and secret key.</returns>
    [HttpGet("[action]")]
    public async Task<ActionResult<AuthAppSetupDto>> TwoFactorSetupData() =>
        this.ProcessResult(await sender.Send(new GetTwoFactorAppSetupDataQry()));

    //------------------------//

    /// <summary>
    /// Completes two-factor authentication app registration via email code.
    /// </summary>
    /// <param name="code">The code sent to the user's email.</param>
    /// <returns>The user's profile if registration is successful.</returns>
    [HttpPost("[action]/{code}")]
    public async Task<ActionResult<AppUserDto>> TwoFactorAuthAppEmailComplete(string code) =>
        this.ProcessResult(await sender.Send(new TwoFactorAuthAppEmailCompleteCmd(code)));

    //------------------------//

    /// <summary>
    /// Completes two-factor authentication app registration.
    /// </summary>
    /// <param name="dto">The registration data for the authenticator app.</param>
    /// <returns>The user's profile if registration is successful.</returns>
    [HttpPost("[action]")]
    public async Task<ActionResult<AppUserDto>> TwoFactorAuthAppComplete([FromBody] TwoFactorAuthAppCompleteRegDto dto) =>
        this.ProcessResult(await sender.Send(new TwoFactorAuthAppCompleteRegCmd(dto)));

    //------------------------//

    /// <summary>
    /// Enables two-factor authentication for the current user.
    /// </summary>
    /// <returns>The user's profile with two-factor enabled.</returns>
    [HttpPatch("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> EnableTwoFactorAuthentication() =>
        this.ProcessResult(await sender.Send(new TwoFactorEnableCmd()));

    //------------------------//

    /// <summary>
    /// Disables two-factor authentication for the current user.
    /// </summary>
    /// <returns>The user's profile with two-factor disabled.</returns>
    [HttpPatch("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> DisableTwoFactorAuthentication() =>
        this.ProcessResult(await sender.Send(new TwoFactorDisableCmd()));

    //------------------------//

    /// <summary>
    /// Resets the password for a user using a reset token.
    /// </summary>
    /// <param name="dto">The reset password data including token and new password.</param>
    /// <returns>A message indicating the result of the password reset.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResetPassword([FromBody] ResetPwdDto dto) =>
           this.ProcessResult(await sender.Send(new ResetPwdCmd(dto)));

}//Cls

