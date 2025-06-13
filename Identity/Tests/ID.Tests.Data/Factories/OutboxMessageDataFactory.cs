using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.OutboxMessages;
using ID.Domain.Utility.Json;
using MassTransit;
using Newtonsoft.Json;
using TestingHelpers;

namespace ID.Tests.Data.Factories;


public static class OutboxMessageDataFactory
{
    private static bool _isProcessed = false;

    //------------------------------------//

    /// <summary>
    /// Alternate Entities will be unprocessed
    /// </summary>
    public static List<IdOutboxMessage> CreateMany(int count = 50)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(DomainEventsFactory.CreateRandom(), id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static IdOutboxMessage Create(
        IIdDomainEvent? ev = null,
        Guid? id = null,
        string? type = null,
        string? error = null,
        DateTime? createdOn = null,
        DateTime? processedOn = null
        )
    {

        id ??= NewId.NextSequentialGuid();
        type ??= $"{RandomStringGenerator.Generate(20)}{id}";
        error ??= $"{RandomStringGenerator.Generate(20)}{id}";
        ev ??= DomainEventsFactory.CreateRandom();
        createdOn ??= RandomDateGenerator.Generate(DateTime.Now.AddDays(-28), DateTime.Now);

        _isProcessed = processedOn != null;

        var paramaters = new[]
           {
            new PropertyAssignment(nameof(IdOutboxMessage.Id),  () => id ),
            new PropertyAssignment(nameof(IdOutboxMessage.Type),  () => type ),
            new PropertyAssignment(nameof(IdOutboxMessage.Error),  () => error ),
            new PropertyAssignment(nameof(IdOutboxMessage.ProcessedOnUtc),  () => _isProcessed ? GetProcessedOn():null ),
            new PropertyAssignment(nameof(IdOutboxMessage.CreatedOnUtc),  () => createdOn ),
            new PropertyAssignment(nameof(IdOutboxMessage.ContentJson),  () => JsonConvert.SerializeObject(
                ev,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new SisoJsonDefaultContractResolver()
                }) ),
        };


            return ConstructorInvoker.CreateNoParamsInstance<IdOutboxMessage>(paramaters);
    }

    //------------------------------------//

    private static DateTime GetProcessedOn() =>
       RandomDateGenerator.Generate(DateTime.Now.AddDays(-10), DateTime.Now);

    //------------------------------------//

}//Cls

