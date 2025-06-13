using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Queries;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Qry.GetTeamMembers;
public record GetMyTeamMembersQry : AIdUserAndTeamAwareQuery<AppUser, List<AppUserDto>>;



