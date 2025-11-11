using ID.Application.AppAbs.Setup;
using ID.GlobalSettings.Constants;
using ID.Persistence.Ef.Postgres.Services;
using ID.Persistence.Ef.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Persistence.Ef.Postgres;

internal static class PersistencePostgresSetup
{

    internal static IServiceCollection AddIdPersistenceEfPostgres(
        this IServiceCollection services,
        IdentityBuilder builder,
        string connectionString)
    {
        services.AddScoped<IIdMigrateService, PostgresDbMigrator>();
        services.AddPersistenceEf(builder, GenerateConfigurationFunction(connectionString));

        services.AddDbContext<IdPostgresMigrationsContext>((sp, config) =>
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
             config.UseNpgsql(connectionString, npgsqlOptions =>
             {
                 npgsqlOptions
                 .EnableRetryOnFailure(3)
                 .MigrationsHistoryTable(IdGlobalConstants.Db.MIGRATIONS_HISTORY_TABLE, IdGlobalConstants.Db.SCHEMA);
                 //npgsqlOptions.MigrationsAssembly(typeof(PnaPostgresMigrationsContext).Assembly.GetName().Name);
             })
             .AddInterceptors(interceptors);

             config.UseSnakeCaseNamingConvention();

             return config;
         };


}//Cls

