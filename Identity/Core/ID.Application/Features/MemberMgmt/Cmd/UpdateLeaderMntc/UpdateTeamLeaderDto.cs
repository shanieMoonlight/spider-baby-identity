namespace ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;

/// <summary>
/// Represents the data required to update the leader of a team.
/// </summary>
/// <remarks>This record is used to specify the new leader's identifier and optionally the team's identifier when
/// updating the team leader.</remarks>
/// <param name="NewLeaderId">The unique identifier of the new team leader. This value is required and cannot be null.</param>
/// <param name="TeamId">The unique identifier of the team whose leader is being updated. This value is optional and can be null if the team
/// context is implied or not required.</param>
public sealed record UpdateTeamLeaderDto(Guid NewLeaderId, Guid TeamId);