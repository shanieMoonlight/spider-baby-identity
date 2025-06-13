using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Claims.Services.Abs;
using ID.Infrastructure.Claims.Services.Imps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Infrastructure.Claims.Services;
internal static class idClaimsSetup
{

    internal static IServiceCollection AddMyIdClaims(this IServiceCollection services)
    {


        services.TryAddTransient<IClaimsBuilderService, ClaimsBuilderService>();
        services.TryAddTransient<IClaimExtraClaimsBuilder, ClaimExtraClaimsBuilder>();

        return services;
    }



}//Cls
