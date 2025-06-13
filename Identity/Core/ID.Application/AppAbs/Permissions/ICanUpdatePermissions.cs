using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.Permissions;
public interface ICanUpdatePermissions<TUser> where TUser : AppUser
{
    Task<GenResult<TUser>> CanUpdateAsync(Guid updateUserId, IIdPrincipalInfoRequest request);
    Task<GenResult<TUser>> CanUpdateAsync(Guid updateUserId, IIdUserAwareRequest<TUser> request);
}