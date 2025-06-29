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

    /// <summary>
    /// Adds a new feature flag to the system.
    /// </summary>
    /// <param name="dto">The feature flag details.</param>
    /// <returns>The created feature flag.</returns>
    [HttpPost]
    public async Task<ActionResult<FeatureFlagDto>> Add([FromBody] FeatureFlagDto dto) =>
        this.ProcessResult(await sender.Send(new CreateFeatureFlagCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates an existing feature flag.
    /// </summary>
    /// <param name="dto">The updated feature flag details.</param>
    /// <returns>The updated feature flag.</returns>
    [HttpPatch]
    public async Task<ActionResult<FeatureFlagDto>> Edit([FromBody] FeatureFlagDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateFeatureFlagCmd(dto)));

    //------------------------//

    /// <summary>
    /// Deletes a feature flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the feature flag to delete.</param>
    /// <returns>The deleted feature flag or a message if not found.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<FeatureFlagDto>> Delete(Guid id) =>
        this.ProcessResult(await sender.Send(new DeleteFeatureFlagCmd(id)));

    //------------------------//

    /// <summary>
    /// Retrieves all feature flags in the system.
    /// </summary>
    /// <returns>A list of all feature flags.</returns>
    [HttpGet]
    public async Task<ActionResult<FeatureFlagDto[]>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllFeatureFlagsQry()));

    //------------------------//

    /// <summary>
    /// Gets a feature flag by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the feature flag.</param>
    /// <returns>The feature flag matching the ID, or NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<FeatureFlagDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetFeatureFlagByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets all feature flags whose names contain the specified value.
    /// </summary>
    /// <param name="name">The name or substring to filter by.</param>
    /// <returns>A list of feature flags whose names contain the value.</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<FeatureFlagDto>>> GetAllByName(string name) =>
        this.ProcessResult(await sender.Send(new GetAllFeatureFlagsByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets all feature flags with the exact specified name.
    /// </summary>
    /// <param name="name">The exact name to filter by.</param>
    /// <returns>A list of feature flags with the exact name.</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<FeatureFlagDto>>> GetByName(string? name) =>
        this.ProcessResult(await sender.Send(new GetFeatureFlagByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of feature flags.
    /// </summary>
    /// <param name="request">Filtering and sorting information.</param>
    /// <returns>A paginated list of feature flags.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<FeatureFlagDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetFeatureFlagsPageQry(request)));


} //Cls