using ID.Application.AppAbs.Setup;
using ID.GlobalSettings.Constants;
using ID.Infrastructure.Persistance.EF.Setup;
using ID.Persistence.Ef.SQL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Persistence.Ef.SQL;

internal static class PersistenceSqlServerSetup
{

    internal static IServiceCollection AddIdPersistenceEfSQL(
        this IServiceCollection services,
        IdentityBuilder builder,
        string connectionString)
    {
        services.AddScoped<IIdMigrateService, SqlDbMigrator>();
        services.AddPersistenceEf(builder, GenerateConfigurationFunction(connectionString));

        services.AddDbContext<IdSqlMigrationsContext>((sp, config) =>
        {
            var interceptors = sp.GetServices<IInterceptor>();
            GenerateConfigurationFunction(connectionString)(config, interceptors);
        });

        return services;
    }

    //--------------------------------//


    internal static Func<DbContextOptionsBuilder, IEnumerable<IInterceptor>, DbContextOptionsBuilder> GenerateConfigurationFunction(string connectionString) =>
        (config, interceptors) =>
        {
            static void providerOptionsAction(SqlServerDbContextOptionsBuilder providerOptions)
            {
                providerOptions
                    .MigrationsHistoryTable(IdGlobalConstants.Db.MIGRATIONS_HISTORY_TABLE, IdGlobalConstants.Db.SCHEMA)
                    .EnableRetryOnFailure(3)
                    .MigrationsAssembly(typeof(IdSqlMigrationsContext).Assembly.GetName().Name);
            }

            config.UseSqlServer(connectionString, providerOptionsAction)
                .AddInterceptors(interceptors);

            return config;
        };


}//Cls

