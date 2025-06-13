using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.Permissions;
public interface ICanDeletePermissions<TUser> where TUser : AppUser
{
    Task<GenResult<TUser>> CanDeleteAsync(Guid? deleteUserId, IIdPrincipalInfoRequest request);
    Task<GenResult<TUser>> CanDeleteAsync(Guid? deleteUserId, IIdUserAndTeamAwareRequest<TUser> request);
}