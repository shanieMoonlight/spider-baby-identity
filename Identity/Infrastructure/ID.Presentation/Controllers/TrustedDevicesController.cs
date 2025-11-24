using ControllerHelpers;
using ID.Application.Authenticators;
using ID.Application.Features.Account.TrustedDevices;
using ID.Application.Features.Account.TrustedDevices.Cmd.Revoke;
using ID.Application.Features.Account.TrustedDevices.Cmd.RevokeByFingerPrint;
using ID.Application.Features.Account.TrustedDevices.Cmd.Trust;
using ID.Application.Features.Account.TrustedDevices.Qry.GetAll;
using ID.Application.Features.Account.TrustedDevices.Qry.GetByFingerprint;
using ID.Application.Features.Account.TrustedDevices.Qry.GetById;
using ID.Application.Features.Account.TrustedDevices.Qry.GetPage;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
[AuthorizedOrDevAuthenticator.ResourceFilter]
public class TrustedDevicesController(ISender sender) : Controller
{

    /// <summary>
    /// Trusts a device for the current user.
    /// </summary>
    /// <param name="dto">Device information required to create the trust (fingerprint, name, user agent, duration).</param>
    /// <returns>The created <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPost]
    //[CanTrustDeviceAuthenticator.ResourceFilter]
    public async Task<ActionResult<TrustedDeviceDto>> Trust([FromBody] TrustDeviceCreateDto dto) =>
        this.ProcessResult(await sender.Send(new TrustDeviceCmd(dto)));

    //--------------------------// 

    /// <summary>
    /// Revoke a trusted device using a full DTO containing the device identifier.
    /// </summary>
    /// <param name="dto">DTO containing the device id to revoke.</param>
    /// <returns>The revoked <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    [HttpPost]
    public async Task<ActionResult<TrustedDeviceDto>> Revoke([FromBody] RevokeTrustedDeviceDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceCmd(dto)));

    //--------------------------// 

    /// <summary>
    /// Revoke a trusted device using a fingerprint supplied in the request body.
    /// </summary>
    /// <param name="dto">DTO containing the fingerprint to revoke.</param>
    /// <returns>The revoked <see cref="TrustedDeviceDto"/> on success, or an error result.</returns>
    //[HttpPatch]
    [HttpPost]
    public async Task<ActionResult<TrustedDeviceDto>> RevokeByFingerprint([FromBody] RevokeTrustedDeviceByFingerprintDto dto) =>
        this.ProcessResult(await sender.Send(new RevokeTrustedDeviceByFingerprintCmd(dto)));

    //--------------------------// 

    /// <summary>
    /// Gets all trusted devices for the current user.
    /// </summary>
    /// <returns>An array of <see cref="TrustedDeviceDto"/> representing the user's trusted devices.</returns>
    [HttpGet]
    public async Task<ActionResult<TrustedDeviceDto[]>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllTrustedDevicesQry()));

    //--------------------------// 

    /// <summary>
    /// Gets a trusted device by identifier.
    /// </summary>
    /// <param name="id">Trusted device identifier.</param>
    /// <returns>The matching <see cref="TrustedDeviceDto"/>, or NotFound if no match exists.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TrustedDeviceDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetTrustedDeviceByIdQry(id)));

    //--------------------------// 

    /// <summary>
    /// Gets trusted devices that match the supplied fingerprint.
    /// </summary>
    /// <param name="fingerprint">Device fingerprint to search for.</param>
    /// <returns>A collection of matching <see cref="TrustedDeviceDto"/> items.</returns>
    [HttpGet("{fingerprint}")]
    public async Task<ActionResult<IEnumerable<TrustedDeviceDto>>> GetByFingerprint(string fingerprint) =>
        this.ProcessResult(await sender.Send(new GetTrustedDeviceByFingerprintQry(fingerprint)));

    //--------------------------// 

    /// <summary>
    /// Returns a paginated list of trusted devices.
    /// </summary>
    /// <param name="request">Optional pagination and filtering request.</param>
    /// <returns>A paginated response containing <see cref="TrustedDeviceDto"/> items.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<TrustedDeviceDto>>> Page(PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetTrustedDevicesPageQry(request)));


} //Cls