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

    /// <summary>
    /// Initializes the identity system, runs migrations, and creates the Super and Maintenance teams and users.
    /// </summary>
    /// <param name="dto">Initialization data including admin credentials and options.</param>
    /// <returns>A message indicating the result of initialization.</returns>
    [HttpPost]
    [InitializedAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> Initialize([FromBody] InitializeDto dto) =>
        this.ProcessResult(await sender.Send(new InitializeCmd(dto)), logger); //Pass logger for extra loggin in Inititalization

    //------------------------//

    /// <summary>
    /// Runs database migrations for the identity system. Requires Super or Dev authorization.
    /// </summary>
    /// <returns>A message indicating the result of the migration.</returns>
    [HttpPost]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> Migrate() =>
        this.ProcessResult(await sender.Send(new MigrateCmd()), logger);//Pass logger for extra loggin in Inititalization

    //------------------------//

    /// <summary>
    /// Returns the public JWT signing key for the system. Requires Super authorization.
    /// </summary>
    /// <returns>The public signing key information.</returns>
    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<PublicSigningKeyDto>> PublicSigningKey() =>
        this.ProcessResult(await sender.Send(new GetPublicSigningKeyCmd()));

    //------------------------//

    /// <summary>
    /// Returns the configured email routes for the system. Requires Super authorization.
    /// </summary>
    /// <returns>The email routes configuration.</returns>
    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<EmailRoutesDto>> EmailRoutes() =>
        this.ProcessResult(await sender.Send(new GetEmailRoutesCmd()));

    //------------------------//

    /// <summary>
    /// Lists the available multi-factor authentication providers.
    /// </summary>
    /// <returns>An array of provider names.</returns>
    [HttpGet]
    public async Task<ActionResult<string[]>> GetTwoFactorProviders() =>
        this.ProcessResult(await sender.Send(new GetProvidersQry()));

    //------------------------//

    /// <summary>
    /// Returns the global application settings. Requires Super or Dev authorization.
    /// </summary>
    /// <returns>The global settings for the identity system.</returns>
    [HttpGet]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public async Task<ActionResult<SettingsDto>> GlobalSettings() =>
        this.ProcessResult(await sender.Send(new GetSettingsCmd()));


}//Cls
