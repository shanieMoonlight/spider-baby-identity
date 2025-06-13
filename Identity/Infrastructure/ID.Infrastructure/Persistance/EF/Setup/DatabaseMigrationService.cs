using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ID.Infrastructure.Persistance.EF.Setup;

/// <summary>
/// Hosted service that automatically runs database migrations in development environment.
/// This replaces the service locator anti-pattern used during startup configuration.
/// </summary>
internal class DatabaseMigrationService(
    IServiceProvider serviceProvider,
    IWebHostEnvironment environment,
    ILogger<DatabaseMigrationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Only run migrations in development environment
        if (!environment.IsDevelopment())
        {
            logger.LogInformation("Skipping database migrations - not in development environment");
            return;
        }

        try
        {
            logger.LogInformation("Starting database migration in development environment");
            
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdDbContext>();
            
            await context.Database.MigrateAsync(cancellationToken);
            
            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to run database migrations during startup");
            throw; // Re-throw to prevent application startup if migrations fail
        }
    }

    //-----------------------//

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // No cleanup needed for migration service
        return Task.CompletedTask;
    }

}//Cls
