using Hangfire;
using ID.Application.Jobs.OutboxMsgs;
using ID.Application.Utility;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.OutboxMsgs;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace ID.Infrastructure.Jobs.Imps.OutboxMsg.Jobs;
internal class ProcessOldOutboxMsgs(IServiceProvider _serviceProvider, ILogger<ProcessOldOutboxMsgs> logger) : AProcess_Old_MyIdOutboxMsgs
{

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Remove Old Outbox Msgs")]
    public override async Task HandleAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();
            IIdentityOutboxMessageRepo _repo = uow.OutboxMessageRepo;
            await _repo.RemoveRangeAsync(new OutboxMsgsRemoveOlderThanSpec());

            await uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogException(e, MyIdLoggingEvents.JOBS.OLD_OUTBOX_MSGS_PROCESSING);
        }

    }
     

}//Cls
