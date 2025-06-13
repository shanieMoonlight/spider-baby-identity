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
    [HttpPost]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> Add([FromBody] TeamDto dto) =>
        this.ProcessResult(await sender.Send(new CreateCustomerTeamCmd(dto)));

    //------------------------//

    [HttpPatch]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> Edit([FromBody] TeamDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTeamCmd(dto)));

    //------------------------//


    [HttpPatch]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> UpdatePositionRange([FromBody] UpdateTeamPositionRangeDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTeamPositionRangeCmd(dto)));

    //------------------------//


    [HttpDelete("{id}")]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> Delete(Guid id) =>
        this.ProcessResult(await sender.Send(new DeleteCustomerTeamCmd(id)));

    //------------------------//

    [HttpGet]
    [MntcMinimumAuthenticator.ResourceFilter(1)]
    public async Task<ActionResult<TeamDto>> GetAll() =>
        this.ProcessResult(await sender.Send(new GetAllTeamsQry()));

    //------------------------//

    /// <summary>
    /// Gets the Super Team 
    /// </summary>
    /// <returns>The Super Team matching the id or NotFound</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<TeamDto>> GetSuper() =>
        this.ProcessResult(await sender.Send(new GetSuperTeamQry()));

    //------------------------//

    /// <summary>
    /// Gets the Maintenance Team 
    /// </summary>
    /// <returns>The Maintenance Team matching the id or NotFound</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<TeamDto>> GetMntc() =>
        this.ProcessResult(await sender.Send(new GetMntcTeamQry()));

    //------------------------//

    /// <summary>
    /// Gets the Team with Id = <paramref name="id"/> 
    /// </summary>
    /// <returns>The Team matching the id or NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> Get(Guid id) =>
        this.ProcessResult(await sender.Send(new GetTeamByIdQry(id)));

    //------------------------//

    /// <summary>
    /// Gets the Team with Name = <paramref name="name"/> 
    /// </summary>
    /// <returns>The Team matching the id or NotFound</returns>
    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetAllByName(string name) =>
        this.ProcessResult(await sender.Send(new GetTeamsByNameQry(name)));

    //------------------------//

    /// <summary>
    /// Gets a paginated list of Teams
    /// </summary>
    /// <param name="request">Filtering and Sorting Info</param>
    /// <returns>Paginated list of Teams</returns>
    [HttpPost]
    public async Task<ActionResult<PagedResponse<TeamDto>>> Page([FromBody] PagedRequest? request) =>
        this.ProcessResult(await sender.Send(new GetTeamsPageQry(request)));

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> AddSubscription([FromBody] AddTeamSubscriptionDto dto) =>
        this.ProcessResult(await sender.Send(new AddTeamSubscriptionCmd(dto)));

    //------------------------//

    [HttpGet("{subId}")]
    [Authorize]
    public async Task<ActionResult<SubscriptionDto>> GetSubscription([FromRoute] Guid subId) =>
        this.ProcessResult(await sender.Send(new GetTeamSubscriptionQry(new GetTeamSubscriptionDto(subId))));

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> RemoveSubscription([FromBody] RemoveTeamSubscriptionDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveTeamSubscriptionCmd(dto)));

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> RecordSubscriptionPayment([FromBody] RecordSubscriptionPaymentDto dto) =>
        this.ProcessResult(await sender.Send(new RecordSubscriptionPaymentCmd(dto)));

    //------------------------//

    [HttpGet("{subId}/{dvcId}")]
    [Authorize]
    public async Task<ActionResult<DeviceDto>> GetDevice([FromRoute] Guid subId, [FromRoute] Guid dvcId) =>
        this.ProcessResult(await sender.Send(new GetDeviceQry(new GetDeviceDto(subId, dvcId))));

    //------------------------//

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> AddDevice([FromBody] AddDeviceToTeamDto dto) =>
        this.ProcessResult(await sender.Send(new AddDeviceToTeamCmd(dto)));

    //------------------------//

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> RemoveDevice([FromBody] RemoveDeviceFromTeamSubscriptionDto dto) =>
        this.ProcessResult(await sender.Send(new RemoveDeviceFromTeamSubscriptionCmd(dto)));

    //------------------------//

    [HttpPatch]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> UpdateDevice([FromBody] DeviceDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateDeviceCmd(dto)));


} //Cls

