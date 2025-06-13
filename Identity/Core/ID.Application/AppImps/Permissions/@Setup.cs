using ID.Application.AppAbs.Permissions;
using ID.Domain.Entities.AppUsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Application.AppImps.Permissions;
internal static class Setup
{
    public static void AddAppPermissions<TUser>(this IServiceCollection services) where TUser : AppUser
    {

        services.TryAddScoped<ICanDeletePermissions<TUser>, CanDeletePermissions<TUser>>();
        services.TryAddScoped<ICanAddPermissions, CanAddPermissions>();
        services.TryAddScoped<ICanChangeLeaderPermissions<TUser>, CanChangeLeaderPermissions<TUser>>();
        services.TryAddScoped<ICanUpdatePermissions<TUser>, CanUpdatePermissions<TUser>>();
        services.TryAddScoped<ICanChangePositionPermissions<TUser>, CanChangePositionPermissions<TUser>>();
        services.TryAddScoped<ICanUpdatePermissions<TUser>, CanUpdatePermissions<TUser>>();
        services.TryAddScoped<ICanViewTeamMemberPermissions<TUser>, CanViewTeamMemberPermissions<TUser>>();

        services.TryAddScoped<IAppPermissionService<TUser>, AppPermissionService<TUser>>();
    }

}
