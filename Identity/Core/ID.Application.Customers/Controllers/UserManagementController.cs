﻿using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Authenticators.Teams;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Customers.Features.MemberMgmt.Cmd.DeleteCustomerMember;
using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomer;
using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomers;
using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomersPage;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;

namespace ID.Application.Customers.Controllers;

[ApiController]
[Route($"{IdRoutes.Base}/[controller]/[action]")]
[Authorize]
public class UserManagementController(ISender sender) : ControllerBase
{

    /// <summary>
    /// Adds a new member to the current customer's team.
    /// </summary>
    /// <param name="dto">The new team member's details.</param>
    /// <returns>The created team member object.</returns>
    [HttpPost]
    public async Task<ActionResult<AppUser_Customer_Dto>> AddCustomerTeamMember([FromBody] AddCustomerMemberDto dto) =>
        this.ProcessResult(await sender.Send(new AddCustomerMemberCmd(dto)));

    //----------------------//

    /// <summary>
    /// Adds a new member to a customer team as a Maintenance team member.
    /// </summary>
    /// <param name="dto">The new team member's details.</param>
    /// <returns>The created team member object.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUser_Customer_Dto>> AddCustomerTeamMemberMntc([FromBody] AddCustomerMember_MntcDto dto) =>
           this.ProcessResult(await sender.Send(new AddCustomerMemberCmd_Mntc(dto)));

    //----------------------//

    /// <summary>
    /// Delete a Customer
    /// </summary>
    /// <param name="userId">User Identifier</param>
    /// <returns>The Deleted Customer</returns>
    [HttpDelete("{userId}")]
    [CustomerAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> DeleteCustomer(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteCustomerMemberCmd(userId)));

    //----------------------//

    [HttpGet]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<IEnumerable<AppUser_Customer_Dto>>> GetCustomers() =>
        this.ProcessResult(await sender.Send(new GetCustomersQry()));

    //----------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<PagedResponse<AppUser_Customer_Dto>>> GetCustomersPage([FromBody] PagedRequest request) =>
        this.ProcessResult(await sender.Send(new GetCustomersPageQry(request)));

    //----------------------//

    /// <summary>
    /// Returns a Customer matching id, <paramref name="userId"/>
    /// </summary>
    /// <returns></returns>   
    [HttpGet("{teamId}/{userId}")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUser_Customer_Dto>> GetCustomer(Guid teamId, Guid userId) =>
        this.ProcessResult(await sender.Send(new GetCustomerQry(teamId, userId)));

    //----------------------//


}//Cls