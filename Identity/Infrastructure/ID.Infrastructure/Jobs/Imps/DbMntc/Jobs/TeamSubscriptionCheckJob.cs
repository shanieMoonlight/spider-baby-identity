using Hangfire;
using ID.Application.Jobs.DbMntc;
using ID.Application.Utility;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Transactions;
using ID.Domain.Entities.AppUsers;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;


namespace ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
internal class TeamSubscriptionCheckJob(IServiceProvider _serviceProvider, ILogger<TeamSubscriptionCheckJob> logger)
    : ATeamSubscriptionCheckJob
{
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Check Expired Subscriptions")]
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {

        using var scope = _serviceProvider.CreateScope();
        var _teamMgr = scope.ServiceProvider.GetRequiredService<IIdentityTeamManager<AppUser>>();
        var transactionService = scope.ServiceProvider.GetRequiredService<IIdentityTransactionService>();
        using var transaction = await transactionService.BeginTransactionAsync(cancellationToken);
        try
        {
            Console.WriteLine("TeamSubscriptionJob:HandleAsync");
            Debug.WriteLine("TeamSubscriptionJob:HandleAsync");


            var teams = await _teamMgr.GetAllTeamsWithExpiredSubscriptions(cancellationToken);

            foreach (var team in teams)
            {
                foreach (var subscription in team.Subscriptions)
                {
                    if (subscription.Expired)
                        subscription.Deactivate();
                }
                await _teamMgr.UpdateAsync(team);
            }

            await transactionService.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogException(e, MyIdLoggingEvents.JOBS.DB_MNTC);
            await transaction.RollbackAsync(cancellationToken);
        }
    }


}//Cls
