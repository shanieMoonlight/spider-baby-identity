using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Authenticators.Teams;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Cmd.Create;
using ID.Application.Features.Teams.Cmd.Delete;
using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
using ID.Application.Features.Teams.Cmd.Dvcs.RemoveDevice;
using ID.Application.Features.Teams.Cmd.Dvcs.UpdateDevice;
using ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
using ID.Application.Features.Teams.Cmd.Subs.RecordSubscriptionPayment;
using ID.Application.Features.Teams.Cmd.Subs.RemoveSubscription;
using ID.Application.Features.Teams.Cmd.Update;
using ID.Application.Features.Teams.Cmd.UpdatePositionRange;
using ID.Application.Features.Teams.Qry.Dvcs.GetDevice;
using ID.Application.Features.Teams.Qry.GetAll;
using ID.Application.Features.Teams.Qry.GetById;
using ID.Application.Features.Teams.Qry.GetByName;
using ID.Application.Features.Teams.Qry.GetMntcTeam;
using ID.Application.Features.Teams.Qry.GetPage;
using ID.Application.Features.Teams.Qry.GetSuperTeam;
using ID.Application.Features.Teams.Qry.Subs.GetSubscription;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;

namespace ID.Presentation.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
[Authorize]
public class TeamsController(ISender sender) : Controller
{
    /// <summary>
    /// Adds a new customer team. Requires Super or Dev authorization.
    /// </summary>
    /// <param name="dto">The team details.</param>
    /// <returns>The created team.</returns>
    [HttpPost]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> Add([FromBody] TeamDto dto) =>
        this.ProcessResult(await sender.Send(new CreateCustomerTeamCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates an existing team. Requires team leader authorization.
    /// </summary>
    /// <param name="dto">The updated team details.</param>
    /// <returns>The updated team.</returns>
    [HttpPatch]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> Edit([FromBody] TeamDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTeamCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates the position range for a team. Requires team leader authorization.
    /// </summary>
    /// <param name="dto">The new position range details.</param>
    /// <returns>The updated team.</returns>
    [HttpPatch]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> UpdatePositionRange([FromBody] UpdateTeamPositionRangeDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTeamPositionRangeCmd(dto)));

    //------------------------//

    /// <summary>
    /// Deletes a customer team by ID. Requires team leader authorization.
    /// </summary>
    /// <param name="id">The ID of the team to delete.</param>
    /// <returns>A message indicating the result of the deletion.</returns>
    [HttpDelete("{id}")]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> Delete(Guid id) =>
        this.ProcessResult(await sender.Send(new DeleteCustomerTeamCmd(id)));

    //------------------------//

    /// <summary>
    /// Retrieves all teams. Requires Maintenance authorization (level 1).
    /// </summary>
    /// <returns>A list of all teams.</returns>
    [HttpGet]
    [MntcMinimumAuthenticator.ResourceFilter(1)]
    public async Task<ActionResult<TeamDto>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllTeamsQry()));

    //------------------------//

    /// <summary>
    /// Gets the Super Team.
    /// </summary>
    /// <returns>The Super Team or NotFound.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<TeamDto>> GetSuper() =>
        this.ProcessResult(await sender.Send(new GetSuperTeamQry()));

    //------------------------//

    /// <summary>
    /// Gets the Maintenance Team.
    /// </summary>
    /// <returns>The Maintenance Team or NotFound.</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<TeamDto>> GetMntc() =>
        this.ProcessResult(await sender.Send(new GetMntcTeamQry()));

    //------------------------//

    /// <summary>
    /// Gets a team by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the team.</param>
    /// <returns>The team matching the ID, or NotFound.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetTeamByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets all teams with the specified name.
    /// </summary>
    /// <param name="name">The name to filter by.</param>
    /// <returns>A list of teams matching the name.</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllByName(string name) =>
        this.ProcessResult(await sender.Send(new GetTeamsByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of teams.
    /// </summary>
    /// <param name="request">Filtering and sorting information.</param>
    /// <returns>A paginated list of teams.</returns>
    [HttpPost]
    public async Task<ActionResult<PagedResponse<TeamDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetTeamsPageQry(request)));

    //------------------------//

    /// <summary>
    /// Adds a subscription to a team. Requires Maintenance authorization.
    /// </summary>
    /// <param name="dto">The subscription details.</param>
    /// <returns>The updated team with the new subscription.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> AddSubscription([FromBody] AddTeamSubscriptionDto dto) =>
        this.ProcessResult(await sender.Send(new AddTeamSubscriptionCmd(dto)));

    //------------------------//

    /// <summary>
    /// Gets a subscription by its unique ID.
    /// </summary>
    /// <param name="subId">The subscription ID.</param>
    /// <returns>The subscription details.</returns>
    [HttpGet("{subId}")]
    [Authorize]
    public async Task<ActionResult<SubscriptionDto>> GetSubscription([FromRoute] Guid subId) =>
        this.ProcessResult(await sender.Send(new GetTeamSubscriptionQry(new GetTeamSubscriptionDto(subId))));

    //------------------------//

    /// <summary>
    /// Removes a subscription from a team. Requires Maintenance authorization.
    /// </summary>
    /// <param name="dto">The subscription removal details.</param>
    /// <returns>The updated team after removal.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> RemoveSubscription([FromBody] RemoveTeamSubscriptionDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveTeamSubscriptionCmd(dto)));

    //------------------------//

    /// <summary>
    /// Records a payment for a team subscription. Requires Maintenance authorization.
    /// </summary>
    /// <param name="dto">The payment details.</param>
    /// <returns>The updated team after recording the payment.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> RecordSubscriptionPayment([FromBody] RecordSubscriptionPaymentDto dto) =>
        this.ProcessResult(await sender.Send(new RecordSubscriptionPaymentCmd(dto)));

    //------------------------//

    /// <summary>
    /// Gets a device by subscription and device ID.
    /// </summary>
    /// <param name="subId">The subscription ID.</param>
    /// <param name="dvcId">The device ID.</param>
    /// <returns>The device details.</returns>
    [HttpGet("{subId}/{dvcId}")]
    [Authorize]
    public async Task<ActionResult<DeviceDto>> GetDevice([FromRoute] Guid subId, [FromRoute] Guid dvcId) =>
        this.ProcessResult(await sender.Send(new GetDeviceQry(new GetDeviceDto(subId, dvcId))));

    //------------------------//

    /// <summary>
    /// Adds a device to a team subscription.
    /// </summary>
    /// <param name="dto">The device details.</param>
    /// <returns>A message indicating the result.</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> AddDevice([FromBody] AddDeviceToTeamDto dto) =>
        this.ProcessResult(await sender.Send(new AddDeviceToTeamCmd(dto)));

    //------------------------//

    /// <summary>
    /// Removes a device from a team subscription.
    /// </summary>
    /// <param name="dto">The device removal details.</param>
    /// <returns>A message indicating the result.</returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> RemoveDevice([FromBody] RemoveDeviceFromTeamSubscriptionDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveDeviceFromTeamSubscriptionCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates a device in a team subscription.
    /// </summary>
    /// <param name="dto">The updated device details.</param>
    /// <returns>A message indicating the result.</returns>
    [HttpPatch]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> UpdateDevice([FromBody] DeviceDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateDeviceCmd(dto)));


} //Cls

