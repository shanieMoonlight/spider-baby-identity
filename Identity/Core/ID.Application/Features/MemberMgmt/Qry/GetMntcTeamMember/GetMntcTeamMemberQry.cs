using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;

namespace ID.Application.Features.MemberMgmt.Qry.GetMntcTeamMember;

/// <summary>
/// Try to get a User with id <paramref name="MemberId"/>  in the Team with <paramref name="TeamId"/>
/// The Principal must be in the team or a Higher team
/// </summary>
/// <param name="MemberId">Member Identifier</param>
public record GetMntcTeamMemberQry(Guid MemberId) : AIdQuery<AppUserDto>;



