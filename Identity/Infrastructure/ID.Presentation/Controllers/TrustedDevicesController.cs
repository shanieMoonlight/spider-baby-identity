using ControllerHelpers;
using ID.Application.Features.Account.Cmd.TrustedDevices;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.RevokeByFingerPrint;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Trust;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
public class TrustedDevicesController(ISender sender, ILogger<TrustedDevicesController> logger) : Controller
{

    [HttpPost]
    public async Task<ActionResult<TrustedDeviceDto>> Trust([FromBody] TrustDeviceCreateDto dto) =>
        this.ProcessResult(await sender.Send(new TrustDeviceCmd(dto)), logger);

    //--------------------------// 

    [HttpPatch]
    public async Task<ActionResult<TrustedDeviceDto>> Revoke([FromBody] RevokeTrustedDeviceDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceCmd(dto)), logger);

    //--------------------------// 

    [HttpPatch]
    public async Task<ActionResult<TrustedDeviceDto>> RevokeByFingerprint([FromBody] RevokeTrustedDeviceByFingerprintDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceByFingerprintCmd(dto)), logger);


} //Cls