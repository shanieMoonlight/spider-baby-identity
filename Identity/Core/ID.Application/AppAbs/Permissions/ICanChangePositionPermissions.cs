using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using MyResults;

namespace ID.Application.AppAbs.Permissions;
public interface ICanChangePositionPermissions<TUser> where TUser : AppUser
{
    Task<GenResult<TUser>> CanChangePositionAsync(Guid? newPositionUserId, int newPosition, IIdUserAndTeamAwareRequest<TUser> request);
}