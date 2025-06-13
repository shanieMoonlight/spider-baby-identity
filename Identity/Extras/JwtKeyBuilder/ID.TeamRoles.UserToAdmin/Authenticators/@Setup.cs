

using ID.TeamRoles.UserToAdmin.Authenticators.Customers;
using ID.TeamRoles.UserToAdmin.Authenticators.Mntc;
using ID.TeamRoles.UserToAdmin.Authenticators.Spr;
using Microsoft.Extensions.DependencyInjection;

namespace ID.TeamRoles.UserToAdmin.Authenticators;
internal static class Setup
{
    /// <summary>
    /// Add all policies to the system
    /// </summary>
    public static IServiceCollection AddTeamRolePolicies(this IServiceCollection services)
    {
        services
            .AddCustomerUserAuthenticatorPolicy()
            .AddCustomerManagerAuthenticatorPolicy()
            .AddCustomerAdminAuthenticatorPolicy()
            .AddCustomerUserMinimumAuthenticatorPolicy()
            .AddCustomerManagerMinimumAuthenticatorPolicy()
            .AddCustomerAdminMinimumAuthenticatorPolicy()
            
            .AddMntcUserAuthenticatorPolicy()
            .AddMntcManagerAuthenticatorPolicy()
            .AddMntcAdminAuthenticatorPolicy()
            .AddMntcUserMinimumAuthenticatorPolicy()
            .AddMntcManagerMinimumAuthenticatorPolicy()
            .AddMntcAdminMinimumAuthenticatorPolicy()
            
            .AddSuperUserAuthenticatorPolicy()
            .AddSuperManagerAuthenticatorPolicy()
            .AddSuperAdminAuthenticatorPolicy()
            .AddSuperUserMinimumAuthenticatorPolicy()
            .AddSuperManagerMinimumAuthenticatorPolicy()
            .AddSuperAdminMinimumAuthenticatorPolicy()
            ;

        return services;
    }

}//Cls
