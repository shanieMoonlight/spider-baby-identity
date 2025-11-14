using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

internal class PendingRetriesHostedService(Channel<PendingRetry> channel, ILogger<PendingRetriesHostedService> logger) 
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await item.Action(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // handle retry/backoff/enqueue again as needed
                logger.LogWarning(ex, "Retry action failed");
            }
        }
    }


    //----------------------------------//

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        // optionally signal completion so reader finishes when shutting down
        channel.Writer.TryComplete();
        return base.StopAsync(cancellationToken);
    }

}//Cls