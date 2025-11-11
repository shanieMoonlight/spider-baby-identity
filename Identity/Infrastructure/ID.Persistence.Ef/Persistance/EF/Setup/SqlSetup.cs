using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Constants;
using ID.Infrastructure.Persistance.EF.Interceptors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StringHelpers;

namespace ID.Infrastructure.Persistance.EF.Setup;


internal static class SqlSetup
{    public static IServiceCollection ConfigureEfSql(this IServiceCollection services, string? connectionString)
    {
        
        if (connectionString.IsNullOrWhiteSpace())
            throw new ArgumentNullException(nameof(connectionString), IDMsgs.Error.Setup.MISSING_CONNECTION_STRING);


        services.AddDbContext<IdDbContext>((sp, config) =>
        {
            var sqlServerExceptionProcessorInterceptor = sp.GetService<SqlServerExceptionProcessorInterceptor>();
            var domainEventToOutboxMsgInterceptor = sp.GetService<DomainEventsToOutboxMsgInterceptor>();
            var environment = sp.GetRequiredService<IWebHostEnvironment>(); // ✅ Resolve through DI when needed

            static void providerOptionsAction(SqlServerDbContextOptionsBuilder providerOptions)
            {
                //providerOptions.EnableRetryOnFailure(3);
                providerOptions.MigrationsHistoryTable(IdGlobalConstants.Db.MIGRATIONS_HISTORY_TABLE, IdGlobalConstants.Db.SCHEMA);
            }

            config.UseSqlServer(connectionString, providerOptionsAction)
                .AddInterceptors(
                    sqlServerExceptionProcessorInterceptor!,
                    domainEventToOutboxMsgInterceptor!
                );



            if (environment?.IsDevelopment() ?? false)
            {
                config
                    .EnableSensitiveDataLogging(true)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }

        });

        return services;
    }

}//Cls
