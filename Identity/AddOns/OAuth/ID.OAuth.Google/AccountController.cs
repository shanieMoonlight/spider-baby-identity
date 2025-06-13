using ControllerHelpers;
using ID.Application.Features.Common.Dtos.User;
using ID.GlobalSettings.Routes;
using ID.OAuth.Google.GoogleSignUp;
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
    public async Task<ActionResult<AppUserDto>> GoogleLogin(GoogleSignInDto dto) =>
        this.ProcessResult(await sender.Send(new GoogleSignInCmd(dto)));

}//Cls