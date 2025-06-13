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

    [HttpDelete("[action]")]
    [CustomerLeaderMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> CloseAccount() =>
        this.ProcessResult(await sender.Send(new CloseMyAccountCmd()));

    //------------------------------------//

    [HttpGet("[action]")]
    [Authorize]
    public async Task<ActionResult<AppUser_Customer_Dto>> MyInfoCustomer() =>
        this.ProcessResult(await sender.Send(new MyInfoCustomerQry()));

    //-----------------------------------------//

    /// <summary>
    /// For customer signups
    /// </summary>
    /// <param name="dto">Customer Info</param>
    /// <returns>Customer object</returns>
    [HttpPost("[action]")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> RegisterCustomer([FromBody] RegisterCustomerDto dto) =>
           this.ProcessResult(await sender.Send(new RegisterCustomerCmd(dto)));

    //-----------------------------------------//

    /// <summary>
    /// For Mntc member to create customer
    /// </summary>
    /// <param name="dto">Customer Info</param>
    /// <returns>Customer object</returns>
    [HttpPost("[action]")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUser_Customer_Dto>> CreateCustomer([FromBody] RegisterCustomer_NoPwdDto dto) =>
           this.ProcessResult(await sender.Send(new RegisterCustomerNoPwdCmd(dto)));

    //-----------------------------------------//
     
    [HttpPost("[action]")]
    public async Task<ActionResult<AppUser_Customer_Dto>> AddCustomerTeamMember([FromBody] AddCustomerMemberDto dto) =>
        this.ProcessResult(await sender.Send(new AddCustomerMemberCmd(dto)));

    //-----------------------------------------//

    [HttpPost("[action]")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUser_Customer_Dto>> AddCustomerTeamMemberMntc([FromBody] AddCustomerMember_MntcDto dto) =>
           this.ProcessResult(await sender.Send(new AddCustomerMemberCmd_Mntc(dto)));

    //-----------------------------------------//

}//Cls

