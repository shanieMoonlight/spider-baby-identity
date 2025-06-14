using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Authenticators.Teams;
using ID.Application.Features.Account.Cmd.AddMntcMember;
using ID.Application.Features.Account.Cmd.AddSprMember;
using ID.Application.Features.Account.Cmd.ConfirmEmail;
using ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;
using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Application.Features.Account.Cmd.Cookies.SignOut;
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
[Route($"{IdRoutes.Base}/{IdRoutes.Account.Controller}")]
[Authorize]
public class AccountController(ISender sender) : ControllerBase
{

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmEmail([FromBody] ConfirmEmailDto dto) =>
        this.ProcessResult(await sender.Send(new ConfirmEmailCmd(dto)));

    //------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmEmailWithPassword([FromBody] ConfirmEmailWithPwdDto dto) =>
    this.ProcessResult(await sender.Send(new ConfirmEmailWithPwdCmd(dto)));

    //------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ForgotPassword([FromBody] ForgotPwdDto dto) =>
        this.ProcessResult(await sender.Send(new ForgotPwdCmd(dto)));

    //------------------------//

    [HttpPost($"{IdRoutes.Account.Actions.Login}")]
    [AllowAnonymous]
    public async Task<ActionResult<JwtPackage>> Login([FromBody] LoginDto dto) =>
           this.ProcessResult(await sender.Send(new LoginCmd(dto)));

    //------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> LoginRefresh([FromBody]  LoginRefreshDto dto ) =>
           this.ProcessResult(await sender.Send(new LoginRefreshCmd(dto.ResetToken, dto.DeviceId)));

    //------------------------//

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
    /// Signs in a user using cookies.
    /// </summary>
    /// <param name="dto">The sign-in data transfer object containing user credentials.</param>
    /// <returns>
    /// A <see cref="MessageResponseDto"/> indicating the result of the sign-in attempt.
    /// Returns a <see cref="PreconditionRequiredResponse"/> if two-factor authentication or email confirmation is required.
    /// Returns <see cref="Unauthorized"/> if the credentials are invalid.
    /// </returns>
    [HttpPost($"{IdRoutes.Account.Actions.CookieSignIn}")]
    [AllowAnonymous]
    public async Task<ActionResult<CookieSignInResultData>> CookieSignIn([FromBody] CookieSignInDto dto) =>
           this.ProcessResult(await sender.Send(new CookieSignInCmd(dto)));

    //------------------------//

    [HttpPost("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ChangePassword([FromBody] ChPwdDto dto) =>
           this.ProcessResult(await sender.Send(new ChPwdCmd(dto)));

    //------------------------//

    [HttpGet("[action]")]
    public async Task<ActionResult<AppUserDto>> MyInfo() =>
        this.ProcessResult(await sender.Send(new MyInfoQry()));

    //------------------------//

    [HttpPost("[action]")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> AddMntcTeamMember([FromBody] AddMntcMemberDto dto) =>
           this.ProcessResult(await sender.Send(new AddMntcMemberCmd(dto)));

    //------------------------//

    [HttpPost("[action]")]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> AddSuperTeamMember([FromBody] AddSprMemberDto dto) =>
        this.ProcessResult(await sender.Send(new AddSprMemberCmd(dto)));

    //------------------------//

    [HttpDelete("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> RefreshTokenRevoke() =>
           this.ProcessResult(await sender.Send(new RefreshTokenRevokeCmd()));

    //------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendEmailConfirmation([FromBody] ResendEmailConfirmationDto dto) =>
           this.ProcessResult(await sender.Send(new ResendEmailConfirmationCmd(dto)));

    //------------------------//

    /// <summary>
    /// Use when already logged in
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ResendEmailConfirmation() =>
        this.ProcessResult(await sender.Send(new ResendEmailConfirmationPrincipalCmd()));

    //------------------------//

    [HttpGet("[action]/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendEmailConfirmation(string email) =>
        this.ProcessResult(await sender.Send(new ResendEmailConfirmationCmd(new() { Email = email })));

    //------------------------//

    /// <summary>
    /// Use when already logged in
    /// </summary>
    [HttpPost($"{IdRoutes.Account.Actions.TwoFactorResend}")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> TwoFactorResend([FromBody] Resend2FactorDto dto) =>
           this.ProcessResult(await sender.Send(new Resend2FactorCmd(dto)));

    //------------------------//

    //[AllowAnonymous]
    [HttpPost($"{IdRoutes.Account.Actions.TwoFactorVerification}")]
    public async Task<ActionResult<JwtPackage>> TwoFactorVerification([FromBody] Verify2FactorDto dto) =>
        this.ProcessResult(await sender.Send(new Verify2FactorCmd(dto)));

    //------------------------//

    //[AllowAnonymous]
    [HttpPost($"{IdRoutes.Account.Actions.TwoFactorVerificationCookie}")]
    public async Task<ActionResult<JwtPackage>> TwoFactorVerificationCookie([FromBody] Verify2FactorCookieDto dto) =>
        this.ProcessResult(await sender.Send(new Verify2FactorCookieCmd(dto)));

    //------------------------//

    [HttpGet("[action]")]
    public async Task<ActionResult<AuthAppSetupDto>> TwoFactorSetupData() =>
        this.ProcessResult(await sender.Send(new GetTwoFactorAppSetupDataQry()));

    //------------------------//

    /// <summary>
    /// Use when already logged in
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]/{code}")]
    public async Task<ActionResult<AppUserDto>> TwoFactorAuthAppEmailComplete(string code) =>
        this.ProcessResult(await sender.Send(new TwoFactorAuthAppEmailCompleteCmd(code)));

    //------------------------//

    /// <summary>
    /// Use when already logged in
    /// </summary>
    /// <returns>AppUser</returns>
    [HttpGet("[action]")]
    public async Task<ActionResult<AppUserDto>> TwoFactorAuthAppComplete([FromBody] TwoFactorAuthAppCompleteRegDto dto) =>
        this.ProcessResult(await sender.Send(new TwoFactorAuthAppCompleteRegCmd(dto)));

    //------------------------//

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> EnableTwoFactorAuthentication() =>
        this.ProcessResult(await sender.Send(new TwoFactorEnableCmd()));

    //------------------------//

    [HttpPatch("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> DisableTwoFactorAuthentication() =>
        this.ProcessResult(await sender.Send(new TwoFactorDisableCmd()));

    //------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResetPassword([FromBody] ResetPwdDto dto) =>
           this.ProcessResult(await sender.Send(new ResetPwdCmd(dto)));

}//Cls

