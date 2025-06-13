using ID.Domain.Abstractions.Events;
using ID.Domain.Utility.Json;
using ID.Domain.Entities.Common;
using MassTransit;
using Newtonsoft.Json;

namespace ID.Domain.Entities.OutboxMessages;
public class IdOutboxMessage : IIdBaseDomainEntity
{
    public Guid Id { get; internal set; }
    public string Type { get; set; } = string.Empty;
    public string ContentJson { get; set; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; set; } = string.Empty;

    //-------------------------------------//

    public static IdOutboxMessage Create(IIdDomainEvent ev)
    {
        return new()
        {
            Id = NewId.NextSequentialGuid(),
            CreatedOnUtc = DateTime.UtcNow,
            Type = ev.GetType().Name,
            ContentJson = JsonConvert.SerializeObject(
                ev,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new SisoJsonDefaultContractResolver()
                }
            ),
        };
    }

    //-------------------------------------//

    public IdOutboxMessage SetProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
        return this;
    }

    //-------------------------------------//

    public override string ToString()
    {
        var nl = Environment.NewLine;
        return $"Id: {Id}{nl}" +
            $"Type: {Type}{nl}" +
            $"CreatedOnUtc: {CreatedOnUtc.ToLongDateString()}{nl}" +
            $"ContentJson: {ContentJson}{nl}" +
            $"ProcessedOnUtc: {ProcessedOnUtc?.ToLongDateString()}{nl}" +
            $"Error: {Error}{nl}"
            ;
    }

    //-------------------------------------//

    private void Investigate(IIdDomainEvent ev)
    {
        var what = JsonConvert.SerializeObject(
                ev,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new SisoJsonDefaultContractResolver()

                }
            );


        var domainEv = JsonConvert.DeserializeObject<IIdDomainEvent>(what, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new SisoJsonDefaultContractResolver()
        });
    }

    //-------------------------------------//

}//Cls
