using ID.Application.Authenticators.Teams;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Application.Authenticators;
internal static class Setup
{
    /// <summary>
    /// Add all policies to the system
    /// </summary>
    public static IServiceCollection AddIdentityPolicies(this IServiceCollection services)
    {
        services
            .AddCustomerAuthenticatorPolicy()
            .AddCustomerLeaderAuthenticatorPolicy()
            .AddCustomerLeaderMinimumPolicy()
            .AddCustomerMinimumAuthenticatorPolicy()

            .AddLeaderAuthenticatorPolicy()

            .AddMntcAuthenticatorPolicy()
            .AddMntcLeaderAuthenticatorPolicy()
            .AddMntcLeaderMinimumAuthenticatorPolicy()
            .AddMntcMinimumAuthenticatorPolicy()
            .AddMntcMinimumOrDevAuthenticatorPolicy()

            .AddPositionMinimumAuthenticatorPolicy()

            .AddSuperAuthenticatorPolicy()
            .AddSuperLeaderAuthenticatorPolicy()
            .AddSuperMinimumAuthenticatorPolicy()
            .AddSuperMinimumOrDevAuthenticatorPolicy()

            .AddDevAccessAuthenticatorPolicy()

            .AddInitializedAuthenticatorPolicy()

            .AddMfaVerifiedAuthenticatorPolicy()
            ;

        return services;
    }

}//Cls
