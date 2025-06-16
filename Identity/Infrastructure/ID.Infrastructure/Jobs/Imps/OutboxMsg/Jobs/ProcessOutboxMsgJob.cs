using Hangfire;
using ID.Application.Jobs.OutboxMsgs;
using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Utility.Json;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Errors;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using LoggingHelpers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ID.Infrastructure.Jobs.Imps.OutboxMsg.Jobs;
internal sealed class ProcessOutboxMsgJob(IServiceProvider _serviceProvider, ILogger<ProcessOutboxMsgJob> logger)
    : AProcessMyIdOutboxMsgJob
{
    //---------------------------------------------//

    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Process Outbox Msgs")]
    public override async Task HandleAsync()
    {
        try
        {

            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
            var repo = uow.OutboxMessageRepo;

            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
     
            var takeSpec = new TakeSkipSpec<IdOutboxMessage>(50, 0, m => m.ProcessedOnUtc == null);
            var msgs = await repo.TakeAsync(takeSpec);
            if (!msgs.Any())
                return;

            foreach (var msg in msgs)
                await ProcessAsync(msg, publisher, uow);

        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.OutboxProcessing);
        }
    }

    //---------------------------------------------//

    private async Task ProcessAsync(IdOutboxMessage msg, IPublisher publisher, IIdUnitOfWork uow)
    {
        IIdDomainEvent? domainEv = null;
        try
        {
            var repo = uow.OutboxMessageRepo;

            domainEv = JsonConvert.DeserializeObject<IIdDomainEvent>(msg.ContentJson, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new SisoJsonDefaultContractResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });

            if (domainEv == null)
            {
                logger.LogError(IdErrorEvents.OutboxProcessing, "{msg}", IDMsgs.Error.Jobs.MISSING_OUTBOX_CONTENT(msg));
                return;
            }

            await publisher.Publish(domainEv);

            msg.SetProcessed();
            await repo.UpdateAsync(msg);
            await uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogException(e, $"Domain Event: {domainEv?.GetType()}", IdErrorEvents.OutboxProcessing);
        }
    }

    //---------------------------------------------//

}//Cls
