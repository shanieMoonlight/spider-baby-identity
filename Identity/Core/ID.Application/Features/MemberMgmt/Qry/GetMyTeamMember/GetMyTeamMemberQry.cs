using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Qry.GetMyTeamMember;


/// <summary>
/// Try to get a User with id <paramref name="MemberId"/>  in the Principals Team
/// The Principal must be in the team also
/// </summary>
/// <param name="MemberId">Member Identifier</param>
public record GetMyTeamMemberQry(Guid MemberId) : AIdUserAndTeamAwareQuery<AppUser, AppUserDto>;



