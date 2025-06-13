using ID.Domain.Entities.OutboxMessages;

namespace ID.Application.Features.OutboxMessages;

public static class OutboxMessageMappings
{
    //------------------------------------//

    public static IdOutboxMessageDto ToDto(this IdOutboxMessage msg) => new()
    {
        Id = msg.Id,
        Type = msg.Type,
        ContentJson = msg.ContentJson,
        CreatedOnUtc = msg.CreatedOnUtc,
        ProcessedOnUtc = msg.ProcessedOnUtc,
        Error = msg.Error,
    };

    //------------------------------------//

}//Cls


