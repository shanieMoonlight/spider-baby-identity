using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppAbs.Permissions;

/// <summary>
/// Basic app permissions for user/team admin. Who can add/delete/update etc.
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IAppPermissionService<TUser> where TUser : AppUser
{
    ICanAddPermissions AddPermissions { get; }
    ICanChangeLeaderPermissions<TUser> ChangeLeaderPermissions { get; }
    ICanChangePositionPermissions<TUser> ChangePositionPermissions { get; }
    ICanDeletePermissions<TUser> DeletePermissions { get; }
    ICanUpdatePermissions<TUser> UpdatePermissions { get; }
    ICanViewTeamMemberPermissions<TUser> ViewTeamMemberPermissions { get; }
}
