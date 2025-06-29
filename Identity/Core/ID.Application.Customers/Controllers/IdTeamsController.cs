using ControllerHelpers;
using ID.Application.Authenticators.Teams;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
using ID.Application.Features.Teams;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ID.Application.Customers.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
[Authorize]
public class TeamsController(ISender sender) : Controller
{

    /// <summary>
    /// Adds a new customer member to a team. Requires Maintenance-level authorization.
    /// </summary>
    /// <param name="dto">The new customer member's details, including team and position.</param>
    /// <returns>The updated team information with the new member added.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> AddCustomerToTeam([FromBody] AddCustomerMember_MntcDto dto) =>
        this.ProcessResult(await sender.Send(new AddCustomerMemberCmd_Mntc(dto)));

} //Cls

