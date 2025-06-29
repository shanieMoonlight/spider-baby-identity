using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Authenticators.Teams;
using ID.Application.Customers.Dtos.User;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
using ID.Application.Customers.Features.Account.Cmd.CloseAccount;
using ID.Application.Customers.Features.Account.Cmd.RegCustomer;
using ID.Application.Customers.Features.Account.Cmd.RegCustomerNoPwd;
using ID.Application.Customers.Features.Account.Qry.MyInfoCustomer;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ID.Application.Customers.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]")]
[Authorize]
public class AccountController(ISender sender) : ControllerBase
{

    /// <summary>
    /// Closes the account for the specified customer team. Requires customer leader authorization.
    /// </summary>
    /// <param name="teamId">The ID of the team whose account will be closed.</param>
    /// <returns>A message indicating the result of the account closure.</returns>
    [HttpDelete("[action]/{teamId}")]
    [CustomerLeaderMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> CloseAccount(Guid teamId) =>
        this.ProcessResult(await sender.Send(new CloseMyAccountCmd(teamId)));

    //------------------------------------//

    /// <summary>
    /// Retrieves the current authenticated customer's profile information.
    /// </summary>
    /// <returns>The current customer's profile as an AppUser_Customer_Dto.</returns>
    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUser_Customer_Dto>> MyInfoCustomer() =>
        this.ProcessResult(await sender.Send(new MyInfoCustomerQry()));

    //-----------------------------------------//

    /// <summary>
    /// Registers a new customer account.
    /// </summary>
    /// <param name="dto">The customer registration information.</param>
    /// <returns>A message indicating the result of the registration.</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> RegisterCustomer([FromBody] RegisterCustomerDto dto) =>
           this.ProcessResult(await sender.Send(new RegisterCustomerCmd(dto)));

    //-----------------------------------------//

    /// <summary>
    /// Creates a new customer account as a Maintenance team member (no password required).
    /// </summary>
    /// <param name="dto">The customer registration information.</param>
    /// <returns>The created customer object.</returns>
    [HttpPost("[action]")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUser_Customer_Dto>> CreateCustomer([FromBody] RegisterCustomer_NoPwdDto dto) =>
           this.ProcessResult(await sender.Send(new RegisterCustomerNoPwdCmd(dto)));

    //-----------------------------------------//
    
    /// <summary>
    /// Adds a new member to the current customer's team.
    /// </summary>
    /// <param name="dto">The new team member's details.</param>
    /// <returns>The created team member object.</returns>
    [HttpPost("[action]")]
    public async Task<ActionResult<AppUser_Customer_Dto>> AddCustomerTeamMember([FromBody] AddCustomerMemberDto dto) =>
        this.ProcessResult(await sender.Send(new AddCustomerMemberCmd(dto)));

    //-----------------------------------------//

    /// <summary>
    /// Adds a new member to a customer team as a Maintenance team member.
    /// </summary>
    /// <param name="dto">The new team member's details.</param>
    /// <returns>The created team member object.</returns>
    [HttpPost("[action]")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUser_Customer_Dto>> AddCustomerTeamMemberMntc([FromBody] AddCustomerMember_MntcDto dto) =>
           this.ProcessResult(await sender.Send(new AddCustomerMemberCmd_Mntc(dto)));

    //-----------------------------------------//

}//Cls

