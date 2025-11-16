using ControllerHelpers;
using ID.Application.Dtos.Account.Cookies;
using ID.Domain.Models;
using ID.GlobalSettings.Routes;
using ID.OAuth.Facebook.FacebookSignUp;
using ID.OAuth.Facebook.Features.SignIn.FacebookCookieSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ID.OAuth.Facebook;

//This controller should be merged with the main AccountController in Presentation Library
[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender) : ControllerBase
{

    /// <summary>
    /// Authenticates a user using Facebook OAuth and returns a JWT package if successful.
    /// </summary>
    /// <param name="dto">The Facebook sign-in data including ID token and optional subscription/device info.</param>
    /// <returns>A JWT package containing access and refresh tokens, or two-factor requirements.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<JwtPackage>> FacebookLogin(FacebookSignInDto dto) =>
        this.ProcessResult(await sender.Send(new FacebookSignInCmd(dto)));

    /// <summary>
    /// Authenticates a user using Facebook OAuth and issues a cookie-based sign-in result.
    /// </summary>
    /// <param name="dto">The Facebook cookie sign-in data including ID token, RememberMe, and optional subscription/device info.</param>
    /// <returns>Sign-in result data, including two-factor or email confirmation requirements if needed.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<CookieSignInResultData>> FacebookCookieSignin(FacebookCookieSignInDto dto) =>
        this.ProcessResult(await sender.Send(new FacebookCookieSignInCmd(dto)));

}//Cls