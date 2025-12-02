using ID.Application.Jobs.Abstractions;
using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.OutboxMsgs;
using ID.Domain.Utility.Json;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;




namespace ID.Application.Jobs.OutboxMsgs;


internal sealed class ProcessMyIdOutboxMsgJob(IServiceProvider _serviceProvider, ILogger<ProcessMyIdOutboxMsgJob> logger)
    : AMyIdJobHandler("OUTBOX_HANDLER")
{

    [MyIdDisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Process Outbox Msgs")]
    public override async Task HandleAsync()
    {
        try
        {

            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
            var repo = uow.OutboxMessageRepo;


            var spec = UnprocessedOutboxMsgsSpec.Create(25);
            var msgs = await repo.ListAllTrackedAsync(spec);
            if (!msgs.Any())
                return;

            foreach (var msg in msgs)
                await ProcessAsync(msg, publisher, uow);

        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Jobs.OutboxProcessing);
        }
    }

    //--------------------------------//

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
                logger.LogError(IdErrorEvents.Jobs.OutboxProcessing, "{msg}", IDMsgs.Error.Jobs.MISSING_OUTBOX_CONTENT(msg));
                return;
            }

            await publisher.Publish(domainEv);

            msg.SetProcessed();
            await repo.UpdateAsync(msg);
            await uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogException(e, $"Domain Event: {domainEv?.GetType()}", IdErrorEvents.Jobs.OutboxProcessing);
        }
    }

}//Cls
