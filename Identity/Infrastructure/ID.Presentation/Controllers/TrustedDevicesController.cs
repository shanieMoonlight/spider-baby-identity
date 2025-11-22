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

/// <summary>
/// API endpoints for managing trusted devices for the current user.
/// </summary>
/// <remarks>
/// Use these endpoints to trust a new device, revoke trusted devices (by id or fingerprint),
/// and to query trusted devices (all, by id, by fingerprint or paginated).
/// All actions are implemented using MediatR commands/queries and return standard
/// ActionResult responses with <see cref="TrustedDeviceDto"/> payloads where appropriate.
/// </remarks>
[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
public class TrustedDevicesController(ISender sender, ILogger<TrustedDevicesController> logger) : Controller
{

    /// <summary>
    /// Trusts a device for the current user.
    /// </summary>
    /// <param name="dto">Device information required to create the trust (fingerprint, name, user agent, duration).</param>
    /// <returns>The created <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPost]
    public async Task<ActionResult<TrustedDeviceDto>> Trust([FromBody] TrustDeviceCreateDto dto) =>
        this.ProcessResult(await sender.Send(new TrustDeviceCmd(dto)), logger);

    //--------------------------// 

    /// <summary>
    /// Revoke a trusted device using a full DTO containing the device identifier.
    /// </summary>
    /// <param name="dto">DTO containing the device id to revoke.</param>
    /// <returns>The revoked <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPatch]
    public async Task<ActionResult<TrustedDeviceDto>> Revoke([FromBody] RevokeTrustedDeviceDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceCmd(dto)), logger);

    //--------------------------// 

    /// <summary>
    /// Revoke a trusted device by its id.
    /// </summary>
    /// <param name="deviceId">The id of the trusted device to revoke.</param>
    /// <returns>The revoked <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPatch("{deviceId}")]
    public async Task<ActionResult<TrustedDeviceDto>> Revoke([FromRoute] Guid deviceId) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceCmd(new RevokeTrustedDeviceDto(deviceId))), logger);

    //--------------------------// 

    /// <summary>
    /// Revoke a trusted device using a fingerprint supplied in the request body.
    /// </summary>
    /// <param name="dto">DTO containing the fingerprint to revoke.</param>
    /// <returns>The revoked <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPatch]
    public async Task<ActionResult<TrustedDeviceDto>> RevokeByFingerprint([FromBody] RevokeTrustedDeviceByFingerprintDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceByFingerprintCmd(dto)), logger);
    //--------------------------// 

    /// <summary>
    /// Revoke a trusted device by fingerprint supplied in the route.
    /// </summary>
    /// <param name="fingerprint">The device fingerprint to revoke.</param>
    /// <returns>The revoked <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPatch("{fingerprint}")]
    public async Task<ActionResult<TrustedDeviceDto>> RevokeByFingerprint([FromRoute] string fingerprint) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceByFingerprintCmd(new RevokeTrustedDeviceByFingerprintDto(fingerprint))), logger);

    //--------------------------// 

    /// <summary>
    /// Gets all trusted devices for the current user.
    /// </summary>
    /// <returns>An array of <see cref="TrustedDeviceDto"/> representing the user's trusted devices.</returns>
    [HttpGet]
    public async Task<ActionResult<TrustedDeviceDto[]>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllTrustedDevicesQry()), logger);

    //--------------------------// 

    /// <summary>
    /// Gets a trusted device by identifier.
    /// </summary>
    /// <param name="id">Trusted device identifier.</param>
    /// <returns>The matching <see cref="TrustedDeviceDto"/>, or NotFound if no match exists.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TrustedDeviceDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetTrustedDeviceByIdQry(id)), logger);

    //--------------------------// 

    /// <summary>
    /// Gets trusted devices that match the supplied fingerprint.
    /// </summary>
    /// <param name="fingerprint">Device fingerprint to search for.</param>
    /// <returns>A collection of matching <see cref="TrustedDeviceDto"/> items.</returns>
    [HttpGet("{fingerprint}")]
    public async Task<ActionResult<IEnumerable<TrustedDeviceDto>>> GetByFingerprint(string fingerprint) =>
        this.ProcessResult(await sender.Send(new GetTrustedDeviceByFingerprintQry(fingerprint)), logger);

    //--------------------------// 

    /// <summary>
    /// Returns a paginated list of trusted devices.
    /// </summary>
    /// <param name="request">Optional pagination and filtering request.</param>
    /// <returns>A paginated response containing <see cref="TrustedDeviceDto"/> items.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<TrustedDeviceDto>>> Page(PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetTrustedDevicesPageQry(request)), logger);

    //--------------------------// 

} //Cls