using ID.Application.AppAbs.Setup;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Interceptors;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Infrastructure.Persistance.EF.Services;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Infrastructure.Persistance.EF.Setup;
public static class PersistenceSetup
{    public static IServiceCollection AddPersistenceEf(
        this IServiceCollection services, IdInfrastructureSetupOptions setupOptions, IdentityBuilder builder)
    {
        var assembly = typeof(PersistenceSetup).Assembly;

        services.ConfigureEfOptions(setupOptions);

        services.AddHttpContextAccessor();

        //Call before the DbContext is registered
        services.SetupInterceptors();
        
        services.ConfigureEfSql(setupOptions.ConnectionString);

        // ✅ Register hosted service for database migrations (replaces service locator anti-pattern)
        services.AddHostedService<DatabaseMigrationService>();

        builder.AddEntityFrameworkStores<IdDbContext>();

        services.SetupRepos();
        services.SetupServices();
        services.AddHealthChecks();

        return services;
    }

    //--------------------------//

    private static IServiceCollection SetupInterceptors(this IServiceCollection services)
    {
        services.AddSingleton<SqlServerExceptionProcessorInterceptor>();
        services.AddSingleton<DomainEventsToOutboxMsgInterceptor>();
        return services;
    }

    //--------------------------//

    private static IServiceCollection SetupRepos(this IServiceCollection services)
    {
        services.TryAddScoped<IIdentityTeamRepo, TeamRepo>();
        services.TryAddScoped<IIdentityMemberAuditRepo<AppUser>, MemberAuditRepo>();
        services.TryAddScoped<IIdentitySubscriptionPlanRepo, SubscriptionPlanRepo>();
        services.TryAddScoped<IIdentityOutboxMessageRepo, OutboxMessageRepo>();
        services.TryAddScoped<IIdentityFeatureFlagRepo, FeatureFlagRepo>();
        services.TryAddScoped<IIdentityRefreshTokenRepo, RefreshTokenRepo>();
        services.TryAddScoped<IIdUnitOfWork, MyIdUnitOfWork>();


        return services;
    }

    //--------------------------//

    private static IServiceCollection SetupServices(this IServiceCollection services)
    {
        services.TryAddScoped<IIdMigrateService, MigrateService>();
        return services;

    }


}//Cls
