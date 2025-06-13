namespace ID.Application.Features.Teams.Qry.Dvcs.GetDevice;
public record GetDeviceDto(Guid SubscriptionId, Guid DeviceId, Guid? TeamId = null);