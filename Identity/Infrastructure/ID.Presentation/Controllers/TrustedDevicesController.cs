using ControllerHelpers;
using ID.Application.Features.Account.Cmd.TrustedDevices;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.RevokeByFingerPrint;
using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Trust;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetAll;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetById;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByName;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetPage;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pagination;

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

    [HttpPatch("{deviceId}")]
    public async Task<ActionResult<TrustedDeviceDto>> Revoke([FromRoute] Guid deviceId) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceCmd(new RevokeTrustedDeviceDto(deviceId))), logger);

    //--------------------------// 

    [HttpPatch]
    public async Task<ActionResult<TrustedDeviceDto>> RevokeByFingerprint([FromBody] RevokeTrustedDeviceByFingerprintDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceByFingerprintCmd(dto)), logger);
    //--------------------------// 

    [HttpPatch("{fingerprint}")]
    public async Task<ActionResult<TrustedDeviceDto>> RevokeByFingerprint([FromRoute] string fingerprint) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceByFingerprintCmd(new RevokeTrustedDeviceByFingerprintDto(fingerprint))), logger);

    [HttpGet]
    public async Task<ActionResult<TrustedDeviceDto[]>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllTrustedDevicesQry()), logger);

    //--------------------------// 

    /// <summary>
    /// Gets the TrustedDevice with Id = <paramref name="id"/> 
    /// </summary>
    /// <returns>The TrustedDevice matching the id or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TrustedDeviceDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetTrustedDeviceByIdQry(id)), logger);

    //--------------------------// 

    /// <summary>
    /// Gets the TrustedDevice with Name = <paramref name="name"/> 
    /// </summary>
    /// <returns>The TrustedDevice matching the id or NotFound</returns>
    [HttpGet("{fingerprint}")]
    public async Task<ActionResult<IEnumerable<TrustedDeviceDto>>> GetByFingerprint(string fingerprint) =>
        this.ProcessResult(await sender.Send(new GetTrustedDeviceByFingerprintQry(fingerprint)), logger);

    //--------------------------// 

    /// <summary>
    /// Gets a paginated list of TrustedDevices
    /// </summary>
    /// <param name="request">Filtering and Sorting Info</param>
    /// <returns>Paginated list of TrustedDevices</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<TrustedDeviceDto>>> Page(PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetTrustedDevicesPageQry(request)), logger);

    //--------------------------// 

} //Cls