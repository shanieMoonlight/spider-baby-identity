using ControllerHelpers;
using ControllerHelpers.Responses;
using ID.Application.Authenticators.Teams;
using ID.Application.Features.Account.Cmd.AddMntcMember;
using ID.Application.Features.Account.Cmd.AddSprMember;
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
[Route($"{IdRoutes.Base}/[controller]/[action]")]
[Authorize]
//[MntcMinimumAuthenticator.ActionFilter]
public class UserManagementController(ISender sender) : ControllerBase
{

    //------------------------//

    /// <summary>
    /// Adds a new member to the Maintenance team. Requires Maintenance-level authorization.
    /// </summary>
    /// <param name="dto">The new member's details.</param>
    /// <returns>The created user's profile.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> AddMntcTeamMember([FromBody] AddMntcMemberDto dto) =>
           this.ProcessResult(await sender.Send(new AddMntcMemberCmd(dto)));

    //------------------------//

    /// <summary>
    /// Adds a new member to the Super team. Requires Super-level authorization.
    /// </summary>
    /// <param name="dto">The new member's details.</param>
    /// <returns>The created user's profile.</returns>
    [HttpPost]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> AddSuperTeamMember([FromBody] AddSprMemberDto dto) =>
        this.ProcessResult(await sender.Send(new AddSprMemberCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates a user's position within their team. Requires Maintenance Leader authorization.
    /// </summary>
    /// <param name="dto">The update position data including user and new position.</param>
    /// <returns>The updated user profile.</returns>
    [HttpPatch]
    [MntcLeaderMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<AppUserDto>> UpdatePosition([FromBody] UpdatePositionDto dto) =>
        this.ProcessResult(await sender.Send(new UpdatePositionCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates the current user's profile information.
    /// </summary>
    /// <param name="dto">The update data for the current user.</param>
    /// <returns>The updated user profile.</returns>
    [HttpPatch]
    public async Task<ActionResult<AppUserDto>> UpdateMember([FromBody] UpdateSelfDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateSelfCmd(dto)));

    //------------------------//

    ///// <summary>
    ///// Set a new TeamLeader
    ///// </summary>
    ///// <param name="newLeaderId">New Leader Identifier</param>
    ///// <returns>The new Leader</returns>
    //[HttpPatch("{newLeaderId}")]
    //[LeaderAuthenticator.ActionFilter]
    //public async Task<ActionResult<TeamDto>> UpdateMyTeamLeader(Guid newLeaderId) =>
    //    this.ProcessResult(await sender.Send(new UpdateMyTeamLeaderCmd(newLeaderId)));

    //------------------------//

    /// <summary>
    /// Updates the leader of a team. Requires Maintenance Leader authorization.
    /// </summary>
    /// <param name="dto">The update team leader data.</param>
    /// <returns>The updated team information.</returns>
    [HttpPatch]
    [MntcLeaderMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<TeamDto>> UpdateLeader([FromBody] UpdateTeamLeaderDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTeamLeaderCmd(dto)));

    /// <summary>
    /// Updates the two-factor authentication provider for the current user.
    /// </summary>
    /// <param name="dto">The new two-factor provider data.</param>
    /// <returns>The updated user profile.</returns>
    [HttpPatch]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> UpdateTwoFactorProvider([FromBody] UpdateTwoFactorProviderDto dto) =>
        this.ProcessResult(await sender.Send(new UpdateTwoFactorProviderCmd(dto)));

    //------------------------//

    /// <summary>
    /// Updates the address for the current user.
    /// </summary>
    /// <param name="address">The new address data.</param>
    /// <returns>The updated user profile.</returns>
    [HttpPatch]
    [Authorize]
    public async Task<ActionResult<AppUserDto>> UpdateAddress([FromBody] IdentityAddressDto address) =>
        this.ProcessResult(await sender.Send(new UpdateAddressCmd(address)));

    //------------------------//

    /// <summary>
    /// Deletes a Maintenance team member by user ID. Requires Maintenance authorization.
    /// </summary>
    /// <param name="userId">The user ID of the member to delete.</param>
    /// <returns>A message indicating the result of the deletion.</returns>
    [HttpDelete("{userId}")]
    [MntcMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<MessageResponseDto>> DeleteMntcMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteMntcMemberCmd(userId)));

    //------------------------//

    /// <summary>
    /// Deletes a Super team member by user ID. Requires Super authorization.
    /// </summary>
    /// <param name="userId">The user ID of the member to delete.</param>
    /// <returns>A message indicating the result of the deletion.</returns>
    [HttpDelete("{userId}")]
    [SuperMinimumAuthenticator.ActionFilter()]
    public async Task<ActionResult<MessageResponseDto>> DeleteSuperMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteSprMemberCmd(userId)));

    //------------------------//

    /// <summary>
    /// Deletes a team member by user ID. Only accessible by the user's own team leader.
    /// </summary>
    /// <param name="userId">The user ID of the member to delete.</param>
    /// <returns>A message indicating the result of the deletion.</returns>
    [HttpDelete("{userId}")]
    public async Task<ActionResult<MessageResponseDto>> DeleteTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new DeleteMyTeamMemberCmd(userId)));

    //------------------------//

    /// <summary>
    /// Retrieves all members of the current user's team.
    /// </summary>
    /// <returns>A list of team members.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUserDto>>> GetTeamMembers() =>
        this.ProcessResult(await sender.Send(new GetMyTeamMembersQry()));

    //------------------------//

    /// <summary>
    /// Retrieves all members of the Maintenance team. Requires Super authorization.
    /// </summary>
    /// <returns>A list of Maintenance team members.</returns>
    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<IEnumerable<AppUserDto>>> GetMntcTeamMembers() =>
        this.ProcessResult(await sender.Send(new GetMntcMembersQry()));

    //------------------------//

    /// <summary>
    /// Retrieves all members of the Super team. Requires Super authorization.
    /// </summary>
    /// <returns>A list of Super team members.</returns>
    [HttpGet]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<IEnumerable<AppUserDto>>> GetSuperTeamMembers() =>
        this.ProcessResult(await sender.Send(new GetSprMembersQry()));

    //------------------------//

    /// <summary>
    /// Retrieves a paged list of Maintenance team members. Requires Maintenance authorization.
    /// </summary>
    /// <param name="request">The paging request parameters.</param>
    /// <returns>A paged response of Maintenance team members.</returns>
    [HttpPost]
    [MntcMinimumAuthenticator.ActionFilter]
    //[AllowAnonymous]
    public async Task<ActionResult<PagedResponse<AppUserDto>>> GetMntcTeamMembersPage([FromBody] PagedRequest request) =>
        this.ProcessResult(await sender.Send(new GetMntcMembersPageQry(request)));

    //------------------------//

    /// <summary>
    /// Retrieves a paged list of Super team members. Requires Super authorization.
    /// </summary>
    /// <param name="request">The paging request parameters.</param>
    /// <returns>A paged response of Super team members.</returns>
    [HttpPost]
    [SuperMinimumAuthenticator.ActionFilter]
    public async Task<ActionResult<PagedResponse<AppUserDto>>> GetSuperTeamMembersPage([FromBody] PagedRequest request) =>
        this.ProcessResult(await sender.Send(new GetSprMembersPageQry(request)));

    //------------------------//

    /// <summary>
    /// Retrieves a team member by user ID if the logged-in user is on the same team.
    /// </summary>
    /// <param name="userId">The user ID of the team member.</param>
    /// <returns>The team member's profile.</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUserDto>> GetMyTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new GetMyTeamMemberQry(userId)));

    //------------------------//
    /// <summary>
    /// Retrieves a team member by team and user ID if the logged-in user is on the same or higher-ranked team.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="userId">The user ID of the team member.</param>
    /// <returns>The team member's profile.</returns>
    [HttpGet("{teamId}/{userId}")]
    public async Task<ActionResult<AppUserDto>> GetTeamMember(Guid teamId, Guid userId) =>
        this.ProcessResult(await sender.Send(new GetTeamMemberQry(teamId, userId)));

    //------------------------//
    /// <summary>
    /// Retrieves a Super team member by user ID. Only accessible by higher-ranked Super team members.
    /// </summary>
    /// <param name="userId">The user ID of the Super team member.</param>
    /// <returns>The Super team member's profile.</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUserDto>> GetSuperTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new GetSuperTeamMemberQry(userId)));

    //------------------------//

    /// <summary>
    /// Retrieves a Maintenance team member by user ID. Only accessible by higher-ranked Super team members.
    /// </summary>
    /// <param name="userId">The user ID of the Maintenance team member.</param>
    /// <returns>The Maintenance team member's profile.</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUserDto>> GetMntcTeamMember(Guid userId) =>
        this.ProcessResult(await sender.Send(new GetMntcTeamMemberQry(userId)));

    //------------------------//


}//Cls