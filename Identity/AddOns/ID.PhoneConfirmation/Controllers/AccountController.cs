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

//This controller will merge with the main AccountController in Presentation Library. 
[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender, ILogger<AccountController> logger) : ControllerBase
{

    //------------------------------------//

    /// <summary>
    /// Confirms a user's phone number using a confirmation code sent to their phone.
    /// </summary>
    /// <param name="dto">The confirmation data including phone and code.</param>
    /// <returns>A message indicating success or failure of phone confirmation.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmPhone(ConfirmPhoneDto dto) =>
        this.ProcessResult(await sender.Send(new ConfirmPhoneCmd(dto)), logger);

    //------------------------------------//

    /// <summary>
    /// Resends the phone confirmation code to the specified user.
    /// </summary>
    /// <param name="dto">The phone confirmation resend data.</param>
    /// <returns>A message indicating whether the confirmation code was sent.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendPhoneConfirmation([FromBody] ResendPhoneConfirmationDto dto) =>
           this.ProcessResult(await sender.Send(new ResendPhoneConfirmationCmd(dto)), logger);

    //------------------------------------//

    /// <summary>
    /// Resends the phone confirmation code to the currently authenticated user.
    /// </summary>
    /// <returns>A message indicating whether the confirmation code was sent.</returns>
    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ResendPhoneConfirmationAuthorized() =>
        this.ProcessResult(await sender.Send(new ResendPhoneConfirmationPrincipalCmd()), logger);

    //------------------------------------//

    /// <summary>
    /// Resends the phone confirmation code to the specified email address (anonymous access).
    /// </summary>
    /// <param name="email">The email address to resend confirmation to.</param>
    /// <returns>A message indicating whether the confirmation code was sent.</returns>
    [HttpGet("[action]/{email}")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendPhoneConfirmation(string email) =>
        this.ProcessResult(await sender.Send(new ResendPhoneConfirmationCmd(new() { Email = email })), logger);

}//Cls