using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Authenticators.Teams;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Features.MemberMgmt.Cmd.DeleteMember;
using ID.Application.Features.MemberMgmt.Cmd.DeleteMntcMember;
using ID.Application.Features.MemberMgmt.Cmd.DeleteSuperMember;
using ID.Application.Features.MemberMgmt.Cmd.UpdateAddress;
using ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;
using ID.Application.Features.MemberMgmt.Cmd.UpdateMyTeamLeader;
using ID.Application.Features.MemberMgmt.Cmd.UpdatePosition;
using ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
using ID.Application.Features.MemberMgmt.Qry.GetMntcTeamMember;
using ID.Application.Features.MemberMgmt.Qry.GetMyTeamMember;
using ID.Application.Features.MemberMgmt.Qry.GetSuperTeamMember;
using ID.Application.Features.MemberMgmt.Qry.GetTeamMemberQry;
using ID.Application.Features.MemberMgmt.Qry.GetTeamMembers;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Qry.GetMntcMembers;
using ID.Application.Features.Teams.Qry.GetMntcMembersPage;
using ID.Application.Features.Teams.Qry.GetSprMembers;
using ID.Application.Features.Teams.Qry.GetSprMembersPage;
using ID.Domain.Models;
using ID.GlobalSettings.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination;

namespace ID.Presentation.Controllers;

[ApiController]
//[Route($"{IdRoutes.Base}/[controller]/[action]")]
[Route($"{IdRoutes.Base}/{IdRoutes.UserManagement.Controller}/[action]")]
[Authorize]
//[MntcMinimumAuthenticator.ActionFilter]
public class UserManagementController(ISender sender) : ControllerBase
{

    //------------------------//
    /// <summary>
    /// Edit a user's position in the team
    /// </summary>
    /// <param name="dto">DTO containing user info</param>
    /// <returns>The Updated User</returns>
    [HttpPatch("{userId}/{newPosition}")]
    [MntcLeaderMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> UpdatePosition(Guid userId, int newPosition) =>
        this.ProcessResult(await sender.Send(new UpdatePositionCmd(userId, newPosition)));

    //------------------------//

    /// <summary>
    /// Edit a user
    /// </summary>
    /// <param name="dto">DTO containing user info</param>
    /// <returns>The Updated User</returns>
    [HttpPatch]
    public async Task<ActionResult<AppUserDto>> UpdateMember([FromBody] UpdateSelfDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateSelfCmd(dto)));

    //------------------------//

    /// <summary>
    /// Set a new TeamLeader
    /// </summary>
    /// <param name="newLeaderId">New Leader Identifier</param>
    /// <returns>The new Leader</returns>
    [HttpPatch("{newLeaderId}")]
    [LeaderAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> UpdateMyTeamLeader(Guid newLeaderId) =>
        this.ProcessResult(await sender.Send(new UpdateMyTeamLeaderCmd(newLeaderId)));

    //------------------------//

    [HttpPatch]
    [MntcLeaderMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> UpdateLeader([FromBody] UpdateTeamLeaderDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTeamLeaderCmd(dto)));

    //------------------------//

    /// <summary>
    /// Edit a user
    /// </summary>
    /// <param name="provider">New TwoFactorProvider</param>
    /// <returns>The Updated User</returns>
    [HttpPatch("{provider}")]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> UpdateTwoFactorProvider(TwoFactorProvider provider) =>
        this.ProcessResult(await sender.Send(new TwoFactorUpdateMethodCmd(provider)));

    //------------------------//

    /// <summary>
    /// Change your address
    /// </summary>
    /// <param name="address">New Address</param>
    /// <returns>The Updated User</returns>
    [HttpPatch]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> UpdateAddress([FromBody] IdentityAddressDto address) =>
        this.ProcessResult(await sender.Send(new UpdateAddressCmd(address)));

    //------------------------//

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">Team member Identifier Identifier</param>
    [HttpDelete("{userId}")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> DeleteMntcMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteMntcMemberCmd(userId)));

    //------------------------//

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userId">Team member Identifier Identifier</param>
    [HttpDelete("{userId}")]
    [SuperMinimumAuthenticator.ActionFilter()]
    public async Task<ActionResult<MessageResponseDto>> DeleteSuperMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteSprMemberCmd(userId)));

    //------------------------//

    /// <summary>
    /// Delete one of your Team Members
    /// </summary>
    /// <param name="userId">Team member Identifier Identifier</param>
    [HttpDelete("{userId}")]
    public async Task<ActionResult<MessageResponseDto>> DeleteTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteMyTeamMemberCmd(userId)));

    //------------------------//

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUserDto>>> GetTeamMembers() =>
        this.ProcessResult(await sender.Send(new GetMyTeamMembersQry()));

    //------------------------//

    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<IEnumerable<AppUserDto>>> GetMntcTeamMembers() =>
        this.ProcessResult(await sender.Send(new GetMntcMembersQry()));

    //------------------------//

    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<IEnumerable<AppUserDto>>> GetSuperTeamMembers() =>
        this.ProcessResult(await sender.Send(new GetSprMembersQry()));

    //------------------------//

    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    //[AllowAnonymous]
    public async Task<ActionResult<PagedResponse<AppUserDto>>> GetMntcTeamMembersPage([FromBody] PagedRequest request) =>
        this.ProcessResult(await sender.Send(new GetMntcMembersPageQry(request)));

    //------------------------//

    //[HttpPost]
    //[SuperMinimumFilter]
    //public ActionResult<PagedResponse<AppUserDto>> GetSuperTeamMembersPage([FromBody] PagedRequest request) =>
    //    this.ProcessResult(GenResult<PagedResponse<AppUserDto>>.UnauthorizedResult("No Man!!!"));

    [HttpPost]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<PagedResponse<AppUserDto>>> GetSuperTeamMembersPage([FromBody] PagedRequest request) =>
        this.ProcessResult(await sender.Send(new GetSprMembersPageQry(request)));

    //------------------------//

    /// <summary>
    /// Returns a TeamMember/AppUSer matching id, <paramref name="userId"/>n 
    /// If the logged in user is on the same team
    /// </summary>
    /// <returns>AppUser</returns>   
    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUserDto>> GetMyTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new GetMyTeamMemberQry(userId)));

    //------------------------//
    /// <summary>
    /// Returns a TeamMember/AppUSer matching id, <paramref name="userId"/>n 
    /// If the logged in user is on the same team or a team with a higher rank
    /// </summary>
    /// <returns>AppUser</returns>   
    [HttpGet("{teamId}/{userId}")]
    public async Task<ActionResult<AppUserDto>> GetTeamMember(Guid teamId, Guid userId) =>
        this.ProcessResult(await sender.Send(new GetTeamMemberQry(teamId, userId)));

    //------------------------//
    /// <summary>
    /// Returns a TeamMember/AppUser matching id, <paramref name="userId"/> if found oin the Super Team
    /// If the logged in user must be also in the Suepr Team and have a higher rank
    /// </summary>
    /// <returns>AppUser</returns>   
    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUserDto>> GetSuperTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new GetSuperTeamMemberQry(userId)));

    //------------------------//

    /// <summary>
    /// Returns a TeamMember/AppUser matching id, <paramref name="userId"/> if found oin the Mntc Team
    /// If the logged in user must be also in the Suepr Team and have a higher rank
    /// </summary>
    /// <returns>AppUser</returns>   
    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUserDto>> GetMntcTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new GetMntcTeamMemberQry(userId)));

    //------------------------//


}//Cls