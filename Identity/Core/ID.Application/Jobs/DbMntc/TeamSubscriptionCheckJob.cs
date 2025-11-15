using ID.Application.Jobs.Abstractions;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Repos.Transactions;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;


namespace ID.Application.Jobs.DbMntc;
internal class TeamSubscriptionCheckJob(IServiceProvider _serviceProvider, ILogger<TeamSubscriptionCheckJob> logger)
    : AMyIdJobHandler("TEAM_SUBSCRIPTIONS_CHECK_JOB")
{
    [MyIdDisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Check Expired Subscriptions")]
    public override async Task HandleAsync()
    {

        using var scope = _serviceProvider.CreateScope();
        var _teamMgr = scope.ServiceProvider.GetRequiredService<IIdentityTeamManager<AppUser>>();
        var transactionService = scope.ServiceProvider.GetRequiredService<IIdentityTransactionService>();

        // Get the execution strategy from the transaction service and run the transactional unit inside it.
        var strategy = await transactionService.CreateExecutionStrategyAsync();

        await strategy.ExecuteAsync(async ct =>
        {
            using var transaction = await transactionService.BeginTransactionAsync(ct);
            try
            {
                var teams = await _teamMgr.GetAllTeamsWithExpiredSubscriptions(ct);

                foreach (var team in teams)
                {
                    foreach (var subscription in team.Subscriptions)
                    {
                        if (subscription.Expired)
                            subscription.Deactivate();
                    }
                    await _teamMgr.UpdateAsync(team);
                }

                await transactionService.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception e)
            {
                logger.LogException(e, IdErrorEvents.Jobs.DbMntc);
                await transaction.RollbackAsync(ct);
            }
        }, default);





    }


}//Cls
