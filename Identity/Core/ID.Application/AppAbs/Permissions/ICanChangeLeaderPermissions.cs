using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using MyResults;

namespace ID.Application.AppAbs.Permissions;
public interface ICanChangeLeaderPermissions<TUser> where TUser : AppUser
{
    Task<GenResult<TUser>> CanChange_MyTeam_LeaderAsync(Guid newLeaderId, IIdUserAndTeamAwareRequest<TUser> request);
    Task<GenResult<Team>> CanChange_SpecifiedTeam_LeaderAsync(Guid? teamId, Guid? newLeaderId, IIdUserAndTeamAwareRequest<TUser> request);
}