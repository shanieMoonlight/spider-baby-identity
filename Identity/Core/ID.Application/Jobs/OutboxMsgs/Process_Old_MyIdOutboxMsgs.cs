using ID.Application.Jobs.Abstractions;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.NewFolder.OutboxMsgs;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;



namespace ID.Application.Jobs.OutboxMsgs;

internal sealed class Process_Old_MyIdOutboxMsgs(
    IServiceProvider _serviceProvider, 
    ILogger<Process_Old_MyIdOutboxMsgs> logger) 
    : AMyIdJobHandler("OLD_OUTBOX_MSGS")
{

    [MyIdDisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Remove Old Outbox Msgs")]
    public override async Task HandleAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();
            IIdentityOutboxMessageRepo _repo = uow.OutboxMessageRepo;
            var spec  = new OutboxMsgsCompletedOlderThanSpec(14);
            await _repo.RemoveRangeAsync(spec);

            await uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Jobs.OutboxProcessing);
        }
    }


}//Cls


