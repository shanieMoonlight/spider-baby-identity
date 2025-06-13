using ID.Application.Authenticators;
using ID.Application.Authenticators.Teams;
using ID.Application.Features.Mntc.Cmd.Init;
using ID.Application.Features.Account.Qry.GetProviders;
using ID.Application.Features.Mntc.Qry.PublicSigningKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Features.Mntc.Qry.EmailRoutes;
using ID.Application.Features.Mntc.Qry.Settings;
using ID.Application.Features.Mntc.Cmd.Migrate;
using ID.GlobalSettings.Routes;


namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
public class IdSystemController(ISender sender, ILogger<IdSystemController> logger) : Controller
{

    [HttpPost]
    [InitializedAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> Initialize(InitializeDto dto) =>
        this.ProcessResult(await sender.Send(new InitializeCmd(dto)), logger); //Pass logger for extra loggin in Inititalization

    //------------------------//

    [HttpPost]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> Migrate() =>
        this.ProcessResult(await sender.Send(new MigrateCmd()), logger);//Pass logger for extra loggin in Inititalization

    //------------------------//

    /// <summary>
    /// Send Client the public JWT key
    /// </summary>
    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<IActionResult> PublicSigningKey() =>
        this.ProcessResult(await sender.Send(new GetPublicSigningKeyCmd()));

    //------------------------//

    /// <summary>
    /// Send Client the public JWT key
    /// </summary>
    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<IActionResult> EmailRoutes() =>
        this.ProcessResult(await sender.Send(new GetEmailRoutesCmd()));

    //------------------------//

    /// <summary>
    /// List of available Multi-Facotor-Auth providers
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<string[]>> GetTwoFactorProviders() =>
        this.ProcessResult(await sender.Send(new GetProvidersQry()));

    //------------------------//

    /// <summary>
    /// Get global application settings
    /// </summary>
    [HttpGet]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public async Task<IActionResult> GlobalSettings() =>
        this.ProcessResult(await sender.Send(new GetSettingsCmd()));


}//Cls
