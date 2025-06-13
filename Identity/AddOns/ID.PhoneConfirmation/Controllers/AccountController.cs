using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.GlobalSettings.Routes;
using ID.PhoneConfirmation.Features.Account.ConfirmPhone;
using ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;
using ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmationPrincipal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace ID.PhoneConfirmation.Controllers;

//This controller should be merged with the main AccountController in Presentation Library
[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender, ILogger<AccountController> logger) : ControllerBase
{

    //------------------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmPhone(ConfirmPhoneDto dto) =>
        this.ProcessResult(await sender.Send(new ConfirmPhoneCmd(dto)), logger);

    //------------------------------------//

    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendPhoneConfirmation([FromBody] ResendPhoneConfirmationDto dto) =>
           this.ProcessResult(await sender.Send(new ResendPhoneConfirmationCmd(dto)), logger);

    //------------------------------------//

    /// <summary>
    /// Use when already logged in
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ResendPhoneConfirmation() =>
        this.ProcessResult(await sender.Send(new ResendPhoneConfirmationPrincipalCmd()), logger);

    //------------------------------------//

    [HttpGet("[action]/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendPhoneConfirmation(string email) =>
        this.ProcessResult(await sender.Send(new ResendPhoneConfirmationCmd(new() { Email = email })), logger);

}//Cls