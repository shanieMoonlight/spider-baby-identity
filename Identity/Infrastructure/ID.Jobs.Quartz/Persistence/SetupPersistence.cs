using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Abs;
using ID.Jobs.Quartz.Persistence.DbUp.Postgres;
using ID.Jobs.Quartz.Persistence.DbUp.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Jobs.Quartz.Persistence;
internal static class SetupPersistence
{

    public static IServiceCollection AddQuartzPersistence(this IServiceCollection services, DatabaseType dbType)
    {


        switch (dbType)
        {
            case DatabaseType.SqlServer:
                services.TryAddScoped<IDbUpMigrator, DbUpSqlServerMigrator>();
                break;
            case DatabaseType.PostgreSql:
                services.TryAddScoped<IDbUpMigrator, DbUpPostgresServerMigrator>();
                break;
            default:
                throw new NotSupportedException($"The database type '{dbType}' is not supported for Quartz job implementations.");
        }


        return services;
    }



}//Cls
