using ID.Application.AppAbs.Setup;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Repos;
using ID.Persistence.Ef.Interceptors;
using ID.Persistence.Ef.Repos;
using ID.Persistence.Ef.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ID.Persistence.Ef.Setup;
public static class PersistenceSetup
{
    public static IServiceCollection AddPersistenceEf(
        this IServiceCollection services,
        IdentityBuilder builder,
        Func<DbContextOptionsBuilder, IEnumerable<IInterceptor>, DbContextOptionsBuilder> configurationFunction
        )
    {
        var assembly = typeof(PersistenceSetup).Assembly;
        services.AddHttpContextAccessor();
        services.AddSingleton<SqlServerExceptionProcessorInterceptor>();
        services.AddSingleton<DomainEventsToOutboxMsgInterceptor>();
        services.AddSingleton<DateTimeNormalizationSaveChangesInterceptor>();
        //services.AddSingleton<ChildEntitySaveChangesInterceptor>();
        //services.AddSingleton<TeamSaveChangesInterceptor>();

        services.AddDbContext<IdDbContext>((sp, config) =>
        {
            var sqlServerExceptionProcessorInterceptor = sp.GetService<SqlServerExceptionProcessorInterceptor>();
            var domainEventToOutboxMsgInterceptor = sp.GetService<DomainEventsToOutboxMsgInterceptor>();
            var dateTimeNormalizationSaveChangesInterceptor = sp.GetService<DateTimeNormalizationSaveChangesInterceptor>();
            //var childInterceptor = sp.GetService<ChildEntitySaveChangesInterceptor>();

            configurationFunction(
                config,
                [
                  sqlServerExceptionProcessorInterceptor!,
                  domainEventToOutboxMsgInterceptor!,
                  dateTimeNormalizationSaveChangesInterceptor!,
                  //childInterceptor!
                ]
            );

            var env = sp.GetService<IWebHostEnvironment>();

            if (env != null && env.IsDevelopment())
            {
                config
                    .EnableSensitiveDataLogging(true)
                    .EnableDetailedErrors(true)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }

        });

        builder.AddEntityFrameworkStores<IdDbContext>();

        services.SetupRepos();
        services.SetupServices();
        services.AddHealthChecks();


        //services.TryAddScoped<ITrustedDeviceService<AppUser>, TrustedDeviceService<AppUser>>();

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
        services.TryAddScoped<IIdentityTrustedDeviceRepo, TrustedDeviceRepo>();
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
