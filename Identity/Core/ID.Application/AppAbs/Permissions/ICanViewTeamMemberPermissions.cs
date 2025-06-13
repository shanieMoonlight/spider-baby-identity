using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.AppAbs.Permissions;
public interface ICanViewTeamMemberPermissions<TUser> where TUser : AppUser
{
    Task<GenResult<TUser>> CanViewTeamMemberAsync(Guid memberTeamId, Guid memberId, IIdUserAwareRequest<TUser> request);
}