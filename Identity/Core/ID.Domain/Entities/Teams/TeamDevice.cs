using ClArch.ValueObjects;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Common;
using MassTransit;
using System.Text.Json;

namespace ID.Domain.Entities.Teams;

/// <summary>
/// PC, Phone etc.
/// </summary>
public class TeamDevice : IdDomainEntity
{
    /// <summary>
    /// Name of device - User defined
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Description of device - To help user identify it later on
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Unique id of device. Generated from Macaddress etc...
    /// </summary>
    public string UniqueId { get; private set; }

    public Guid SubscriptionId { get; private set; }
    public TeamSubscription? Subscription { get; private set; }


    //------------------------//

    public string Serialize() => JsonSerializer.Serialize(this);

    //------------------------//

    #region EfCoreCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private TeamDevice() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - //

    private TeamDevice(
        TeamSubscription sub,
        Name name,
        DescriptionNullable description,
        UniqueId uniqueId)
        : base(NewId.NextSequentialGuid())
    {
        Name = name.Value;
        Description = description.Value ?? string.Empty;
        UniqueId = uniqueId.Value;
        SubscriptionId = sub.Id;
        Subscription = sub;


        //RaiseDomainEvent(new DeviceCreatedDomainEvent(Id, this));
    }

    //------------------------//

    internal static TeamDevice Create(
        TeamSubscription sub,
        Name name,
        DescriptionNullable description,
        UniqueId uniqueId)
    {
        var user = new TeamDevice(
           sub,
           name,
           description,
           uniqueId);

        return user;
    }

    //- - - - - - - - - - - - //   

    public TeamDevice Update(
        Name name,
        DescriptionNullable description)
    {
        Name = name.Value;
        Description = description.Value ?? string.Empty;

        //RaiseDomainEvent(new DeviceUpdatedDomainEvent(Id, this));

        return this;
    }

    //------------------------//

    #region EquasAndHashCode

    public override bool Equals(object? thatObj) =>
        thatObj is TeamDevice that 
        && UniqueId == that.UniqueId
        && SubscriptionId == that.SubscriptionId;

    //- - - - - - - - - - - - - - - - - - - - - //   

    public override int GetHashCode() =>
        HashCode.Combine(UniqueId, SubscriptionId);

    #endregion

    //------------------------// 


}//Cls


