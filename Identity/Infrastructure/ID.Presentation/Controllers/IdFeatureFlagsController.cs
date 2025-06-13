using ControllerHelpers;
using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Cmd.Create;
using ID.Application.Features.FeatureFlags.Cmd.Delete;
using ID.Application.Features.FeatureFlags.Cmd.Update;
using ID.Application.Features.FeatureFlags.Qry.GetAll;
using ID.Application.Features.FeatureFlags.Qry.GetAllByName;
using ID.Application.Features.FeatureFlags.Qry.GetById;
using ID.Application.Features.FeatureFlags.Qry.GetByName;
using ID.Application.Features.FeatureFlags.Qry.GetPage;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;

namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
public class IdFeatureFlagsController(ISender sender) : Controller
{

    //------------------------//

    [HttpPost]
    public async Task<ActionResult<FeatureFlagDto>> Add(FeatureFlagDto dto) =>
        this.ProcessResult(await sender.Send(new CreateFeatureFlagCmd(dto)));

    //------------------------//

    [HttpPatch]
    public async Task<ActionResult<FeatureFlagDto>> Edit(FeatureFlagDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateFeatureFlagCmd(dto)));

    //------------------------//

    [HttpDelete("{id}")]
    public async Task<ActionResult<FeatureFlagDto>> Delete(Guid id) =>
        this.ProcessResult(await sender.Send(new DeleteFeatureFlagCmd(id)));

    //------------------------//

    [HttpGet]
    public async Task<ActionResult<FeatureFlagDto[]>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllFeatureFlagsQry()));

    //------------------------//

    /// <summary>
    /// Gets the FeatureFlag with Id = <paramref name="id"/> 
    /// </summary>
    /// <returns>The FeatureFlag matching the id or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<FeatureFlagDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetFeatureFlagByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets all the FeatureFlags with Name containing <paramref name="name"/> 
    /// </summary>
    /// <returns>The FeatureFlag matching the id or NotFound</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<FeatureFlagDto>>> GetAllByName(string name) =>
        this.ProcessResult(await sender.Send(new GetAllFeatureFlagsByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets the FeatureFlag with Name = <paramref name="name"/> 
    /// </summary>
    /// <returns>The FeatureFlag matching the id or NotFound</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<FeatureFlagDto>>> GetByName(string? name) =>
        this.ProcessResult(await sender.Send(new GetFeatureFlagByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of FeatureFlags
    /// </summary>
    /// <param name="request">Filtering and Sorting Info</param>
    /// <returns>Paginated list of FeatureFlags</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<FeatureFlagDto>>> Page(PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetFeatureFlagsPageQry(request)));


} //Cls