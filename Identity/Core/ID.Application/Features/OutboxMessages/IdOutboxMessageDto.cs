namespace ID.Application.Features.OutboxMessages;
public class IdOutboxMessageDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ContentJson { get; set; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; } = string.Empty;

}

