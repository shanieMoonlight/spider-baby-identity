namespace ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
public record AddDeviceToTeamDto(
    Guid SubscriptionId,
    string Name,
    string? Description,
    string UniqueId);