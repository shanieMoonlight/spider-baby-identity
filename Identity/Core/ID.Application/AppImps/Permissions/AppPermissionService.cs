using ID.Application.AppAbs.Permissions;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppImps.Permissions;
internal class AppPermissionService<TUser>(
    ICanAddPermissions _addPermissions,
    ICanChangeLeaderPermissions<TUser> _changeLeaderPermissions,
    ICanChangePositionPermissions<TUser> _changePositionPermissions,
    ICanDeletePermissions<TUser> _deletePermissions,
    ICanUpdatePermissions<TUser> _updatePermissions,
    ICanViewTeamMemberPermissions<TUser> _viewTeamMemberPermissions)
    : IAppPermissionService<TUser> where TUser : AppUser
{

    public ICanAddPermissions AddPermissions { get; } = _addPermissions;

    //- - - - - - - - - - - - - - - - - - //

    public ICanChangeLeaderPermissions<TUser> ChangeLeaderPermissions { get; } = _changeLeaderPermissions;

    //- - - - - - - - - - - - - - - - - - //

    public ICanChangePositionPermissions<TUser> ChangePositionPermissions { get; } = _changePositionPermissions;

    //- - - - - - - - - - - - - - - - - - //

    public ICanDeletePermissions<TUser> DeletePermissions { get; } = _deletePermissions;

    //- - - - - - - - - - - - - - - - - - //

    public ICanUpdatePermissions<TUser> UpdatePermissions { get; } = _updatePermissions;

    //- - - - - - - - - - - - - - - - - - //

    public ICanViewTeamMemberPermissions<TUser> ViewTeamMemberPermissions { get; } = _viewTeamMemberPermissions;

}//Int
