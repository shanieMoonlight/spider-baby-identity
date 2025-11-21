using ControllerHelpers;
using ID.Application.Authenticators;
using ID.Application.Features.DevMode.DeleteSubscriptionPlans;
using ID.Application.Features.DevMode.SeedSubscriptionPlans;
using ID.Application.Features.System.Qry.Settings;
using ID.Domain.Entities.AppUsers;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace ID.Presentation.Controllers;

[ApiController]
[DevAccessAuthenticator.ActionFilter]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
public class IdDevelopmentController(ISender sender) : Controller
{

    /// <summary>
    /// Seeds some sample SubscriptionPlans for development purposes. Requires Dev authorization.
    /// </summary>
    /// <returns>The global settings for the identity system.</returns>
    [HttpGet]
    public async Task<ActionResult<SettingsDto>> SeedSubscriptionPlans() =>
        this.ProcessResult(await sender.Send(new SeedSubscriptionPlansCmd<AppUser>()));


    //-------------------------//


    /// <summary>
    /// Deletes all SubscriptionPlans. For development purposes. Requires Dev authorization.
    /// </summary>
    /// <returns>The global settings for the identity system.</returns>
    [HttpGet]
    public async Task<ActionResult<SettingsDto>> DeleteSubscriptionPlans() =>
        this.ProcessResult(await sender.Send(new DeleteSubscriptionPlansCmd<AppUser>()));


}//Cls
