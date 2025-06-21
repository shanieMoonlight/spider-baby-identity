using ControllerHelpers;
using ID.Application.Dtos.Account.Cookies;
using ID.Domain.Models;
using ID.GlobalSettings.Routes;
using ID.OAuth.Google.Features.SignIn;
using ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;
using ID.OAuth.Google.Features.SignIn.GoogleSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ID.OAuth.Google;

//This controller should be merged with the main AccountController in Presentation Library
[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender) : ControllerBase
{

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<JwtPackage>> GoogleLogin(GoogleSignInDto dto) =>
        this.ProcessResult(await sender.Send(new GoogleSignInCmd(dto)));



    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<CookieSignInResultData>> GoogleCookieSignin(GoogleCookieSignInDto dto) =>
        this.ProcessResult(await sender.Send(new GoogleCookieSignInCmd(dto)));

}//Cls