using ID.Domain.Entities.Teams;

namespace ID.Domain.Utility.Exceptions;
public sealed class DeviceLimitExceededException(TeamSubscription sub) : MyIdException(
@$"The device limit of Subscription {sub.Name} is {sub.DeviceLimit} has already been reached. 
Remove a device from the Subscription before adding a new one.")
{ }
