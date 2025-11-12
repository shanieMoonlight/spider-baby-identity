using Hangfire;
using ID.Infrastructure.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ID.Infrastructure.Jobs.Service.HangFire;

internal sealed class IsolatedHangfireServer(JobStorage jobStorage, ILogger<IsolatedHangfireServer> logger) : IHostedService, IDisposable
{
    private readonly JobStorage _jobStorage = jobStorage ?? throw new ArgumentNullException(nameof(jobStorage));
    private readonly ILogger<IsolatedHangfireServer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private BackgroundJobServer? _server;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting isolated Hangfire server for MyId.");

        var serverOptions = new BackgroundJobServerOptions
        {
            ServerName = IdInfrastructureConstants.Jobs.Server,
            Queues = IdInfrastructureConstants.Jobs.Queues.All
        };

        // Start a BackgroundJobServer bound only to the provided JobStorage.
        _server = new BackgroundJobServer(serverOptions, _jobStorage);

        _logger.LogInformation("Isolated Hangfire server started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping isolated Hangfire server for MyId.");
        _server?.Dispose();
        _server = null;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _server?.Dispose();
        _server = null;
    }
}