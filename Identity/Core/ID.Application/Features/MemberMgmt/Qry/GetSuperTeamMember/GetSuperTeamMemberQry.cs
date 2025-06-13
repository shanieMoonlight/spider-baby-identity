using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Qry.GetSuperTeamMember;

/// <summary>
/// Try to get a User with id <paramref name="MemberId"/>  in the Team with <paramref name="TeamId"/>
/// The Principal must be in the team or a Higher team
/// </summary>
/// <param name="TeamId">Team Identifier</param>
/// <param name="MemberId">Member Identifier</param>
public record GetSuperTeamMemberQry(Guid MemberId) : AIdUserAndTeamAwareQuery<AppUser, AppUserDto>;



