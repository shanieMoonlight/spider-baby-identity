using ID.Application.Jobs.Abstractions;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.TrustedDevices;
using ID.Domain.Repos.Transactions;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;



namespace ID.Application.Jobs.DbMntc;
internal class ExpiredTrustedDevicesJob(IServiceProvider _serviceProvider, ILogger<ExpiredTrustedDevicesJob> logger)
    : AMyIdJobHandler("EXPIRED_TRUSTED_DEVICES_JOB")
{
    [MyIdDisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Check Expired Trusted Devices")]
    public override async Task HandleAsync()
    {

        using var scope = _serviceProvider.CreateScope();
        var _repo = scope.ServiceProvider.GetRequiredService<IIdentityTrustedDeviceRepo>();
        var transactionService = scope.ServiceProvider.GetRequiredService<IIdentityTransactionService>();

        // Get the execution strategy from the transaction service and run the transactional unit inside it.
        var strategy = await transactionService.CreateExecutionStrategyAsync();

        await strategy.ExecuteAsync(async ct =>
        {
            using var transaction = await transactionService.BeginTransactionAsync(ct);
            try
            {
                var spec = TrustedDevicesExpiredSpec.Create(expiredByDays: IdGlobalConstants.TrustedDevices.MAX_EXPIRED_BY_DAYS);

                var devices = await _repo.ListAllTrackedAsync(spec, ct);


                var batches = devices.Chunk(50).ToList();

                foreach (var batch in batches)
                {
                    await _repo.RemoveRangeAsync(batch);
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
