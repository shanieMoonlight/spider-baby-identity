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

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
[MntcMinimumAuthenticator.ActionFilter]
public class IdSubscriptionPlansController(ISender sender) : Controller
{

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(3)]
    public async Task<ActionResult<SubscriptionPlanDto>> Add([FromBody] SubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new CreateSubscriptionPlanCmd(dto)));

    //------------------------//

    [HttpPatch]
    [MntcMinimumAuthenticator.ActionFilter(3)]
    public async Task<ActionResult<SubscriptionPlanDto>> Edit([FromBody] SubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateSubscriptionPlanCmd(dto)));

    //------------------------//

    [HttpDelete("{id}")]
    [MntcMinimumAuthenticator.ActionFilter(4)]
    public async Task<IActionResult> Delete(Guid id) =>
        this.ProcessResult(await sender.Send(new DeleteSubscriptionPlanCmd(id)));

    //------------------------//

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllSubscriptionPlansQry()));

    //------------------------//

    /// <summary>
    /// Gets the SubscriptionPlan with Id = <paramref name="id"/> 
    /// </summary>
    /// <returns>The SubscriptionPlan matching the id or NotFound</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<SubscriptionPlanDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetSubscriptionPlanByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets the SubscriptionPlan with Id = <paramref name="name"/> 
    /// </summary>
    /// <returns>The First SubscriptionPlan matching the id or NotFound</returns>
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<SubscriptionPlanDto>> GetByName(string name) =>
        this.ProcessResult(await sender.Send(new GetSubscriptionPlanByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets the SubscriptionPlan with Name = <paramref name="name"/> 
    /// </summary>
    /// <returns>The SubscriptionPlan matching the id or NotFound</returns>
    [HttpGet("{name}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetAllByName(string name) =>
        this.ProcessResult(await sender.Send(new GetAllSubscriptionPlansFilteredQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of SubscriptionPlans
    /// </summary>
    /// <param name="request">Filtering and Sorting Info</param>
    /// <returns>Paginated list of SubscriptionPlans</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<SubscriptionPlanDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetSubscriptionPlansPageQry(request)));

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> AddFeature([FromBody] AddFeatureToPlanDto dto) =>
        this.ProcessResult(await sender.Send(new AddFeatureToSubscriptionPlanCmd(dto.ToMultipleFeaturesDto())));

    //- - - - - - - - - - - - - - - - - - //

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> AddFeatures([FromBody] AddFeaturesToPlanDto dto) =>
        this.ProcessResult(await sender.Send(new AddFeatureToSubscriptionPlanCmd(dto)));

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> RemoveFeature([FromBody] RemoveFeatureFromSubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveFeaturesFromSubscriptionPlanCmd(dto.ToMultipleFeaturesDto())));

    //- - - - - - - - - - - - - - - - - - //

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter(1)]
    public async Task<ActionResult<SubscriptionPlanDto>> RemoveFeatures([FromBody] RemoveFeaturesFromSubscriptionPlanDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveFeaturesFromSubscriptionPlanCmd(dto)));

    //------------------------//

} //Cls

