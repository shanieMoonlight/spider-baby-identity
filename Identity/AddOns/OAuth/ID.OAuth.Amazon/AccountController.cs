using ControllerHelpers;
using ID.Application.Dtos.Account.Cookies;
using ID.Domain.Models;
using ID.GlobalSettings.Routes;
using ID.OAuth.Amazon.Features.SignIn;
using ID.OAuth.Amazon.Features.SignIn.AmazonCookieSignIn;
using ID.OAuth.Amazon.Features.SignIn.AmazonSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ID.OAuth.Amazon;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender) : ControllerBase
{
    // TODO: Add Amazon sign-in command/handler and DTOs under Features similar to Facebook

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<JwtPackage>> AmazonLogin(AmazonSignInDto dto ) =>
        this.ProcessResult(await sender.Send(new AmazonSignInCmd(dto)));



    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<CookieSignInResultData>> AmazonCookieSignin(AmazonCookieSignInDto dto) =>
        this.ProcessResult(await sender.Send(new AmazonCookieSignInCmd(dto)));

}//Cls
