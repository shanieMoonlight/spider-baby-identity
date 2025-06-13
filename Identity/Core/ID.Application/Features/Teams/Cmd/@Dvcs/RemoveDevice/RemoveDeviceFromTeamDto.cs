namespace ID.Application.Features.Teams.Cmd.Dvcs.RemoveDevice;
public record RemoveDeviceFromTeamSubscriptionDto(Guid SubscriptionId, Guid DeviceId);
//Will use UserInfo.TeamId 
//Can only add/remove from own team