using ID.Application.Authenticators.Teams;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;
using ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;
using ID.Application.Features.SubscriptionPlans.Cmd.Create;
using ID.Application.Features.SubscriptionPlans.Cmd.Delete;
using ID.Application.Features.SubscriptionPlans.Cmd.Update;
using ID.Application.Features.SubscriptionPlans.Qry.GetAll;
using ID.Application.Features.SubscriptionPlans.Qry.GetById;
using ID.Application.Features.SubscriptionPlans.Qry.GetByName;
using ID.Application.Features.SubscriptionPlans.Qry.GetFiltered;
using ID.Application.Features.SubscriptionPlans.Qry.GetPage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;
using ControllerHelpers;
using ID.GlobalSettings.Routes;

namespace ID.Presentation.Controllers;


/// <summary>
/// Work in progress. This may change in the future.
/// </summary>

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
[MntcMinimumAuthenticator.ActionFilter]
public class IdSubscriptionPlansController(ISender sender) : Controller
{

    /// <summary>
    /// Adds a new subscription plan. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <param name="dto">The subscription plan details.</param>
    /// <returns>The created subscription plan.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> Add([FromBody] SubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new CreateSubscriptionPlanCmd(dto)));

    //------------------------//

    /// <summary>
    /// Edits an existing subscription plan. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <param name="dto">The updated subscription plan details.</param>
    /// <returns>The updated subscription plan.</returns>
    [HttpPatch]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> Edit([FromBody] SubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateSubscriptionPlanCmd(dto)));

    //------------------------//

    /// <summary>
    /// Deletes a subscription plan by ID. Requires Maintenance authorization (level 2).
    /// </summary>
    /// <param name="id">The ID of the subscription plan to delete.</param>
    /// <returns>A message indicating the result of the deletion.</returns>
    [HttpDelete("{id}")]
    [MntcMinimumAuthenticator.ActionFilter(2)]
    public async Task<IActionResult> Delete(Guid id) =>
        this.ProcessResult(await sender.Send(new DeleteSubscriptionPlanCmd(id)));

    //------------------------//

    /// <summary>
    /// Retrieves all subscription plans.
    /// </summary>
    /// <returns>A list of all subscription plans.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllSubscriptionPlansQry()));

    //------------------------//

    /// <summary>
    /// Gets a subscription plan by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the subscription plan.</param>
    /// <returns>The subscription plan matching the ID, or NotFound.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<SubscriptionPlanDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetSubscriptionPlanByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets the first subscription plan matching the specified name.
    /// </summary>
    /// <param name="name">The name of the subscription plan.</param>
    /// <returns>The first subscription plan matching the name, or NotFound.</returns>
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<SubscriptionPlanDto>> GetByName(string name) =>
        this.ProcessResult(await sender.Send(new GetSubscriptionPlanByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets all subscription plans matching the specified name.
    /// </summary>
    /// <param name="name">The name to filter by.</param>
    /// <returns>A list of subscription plans matching the name.</returns>
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetAllByName(string name) =>
        this.ProcessResult(await sender.Send(new GetAllSubscriptionPlansFilteredQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of subscription plans.
    /// </summary>
    /// <param name="request">Filtering and sorting information.</param>
    /// <returns>A paginated list of subscription plans.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<SubscriptionPlanDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetSubscriptionPlansPageQry(request)));

    //------------------------//

    /// <summary>
    /// Adds a feature to a subscription plan. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <param name="dto">The feature and plan details.</param>
    /// <returns>The updated subscription plan.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> AddFeature([FromBody] AddFeatureToPlanDto dto) =>
        this.ProcessResult(await sender.Send(new AddFeatureToSubscriptionPlanCmd(dto.ToMultipleFeaturesDto())));

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Adds multiple features to a subscription plan. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <param name="dto">The features and plan details.</param>
    /// <returns>The updated subscription plan.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> AddFeatures([FromBody] AddFeaturesToPlanDto dto) =>
        this.ProcessResult(await sender.Send(new AddFeatureToSubscriptionPlanCmd(dto)));

    //------------------------//

    /// <summary>
    /// Removes a feature from a subscription plan. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <param name="dto">The feature and plan details.</param>
    /// <returns>The updated subscription plan.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> RemoveFeature([FromBody] RemoveFeatureFromSubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveFeaturesFromSubscriptionPlanCmd(dto.ToMultipleFeaturesDto())));

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Removes multiple features from a subscription plan. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <param name="dto">The features and plan details.</param>
    /// <returns>The updated subscription plan.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> RemoveFeatures([FromBody] RemoveFeaturesFromSubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveFeaturesFromSubscriptionPlanCmd(dto)));

    //------------------------//

} //Cls

