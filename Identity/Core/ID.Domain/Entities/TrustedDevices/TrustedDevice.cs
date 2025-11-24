using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.TrustedDevices.Events;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using MassTransit;

namespace ID.Domain.Entities.TrustedDevices;

public class TrustedDevice : IdDomainEntity
{
    public Guid UserId { get; private set; }
    public AppUser? User { get; private set; }

    public string Fingerprint { get; private set; }
    public string? Name { get; private set; }
    public string UserAgent { get; private set; }
    public string IpAddress { get; private set; }

    /// <summary>
    /// Null is indefinite trust
    /// </summary>
    public DateTime? TrustedUntil { get; private set; }

    public DateTime LastUsedDate { get; private set; }


    /// <summary>
    /// Trusted devices for this user
    /// </summary>
    public IReadOnlyCollection<IdRefreshToken> IdRefreshTokens { get; private set; }



    #region EfCoreCtor
    // Used by EF Core
#pragma warning disable CS8618
    private TrustedDevice() { }
#pragma warning restore CS8618
    #endregion

    //- - - - - - - - - - - - //

    private TrustedDevice(
        AppUser user,
        DeviceFingerprint fingerprint,
        DeviceName name,
        UserAgent userAgent,
        IpAddress ipAddress,
        DateTime? trustedUntil)
        //: base()
        : base(NewId.NextSequentialGuid())
    {
        UserId = user.Id;
        User = user;
        Fingerprint = fingerprint.Value;
        Name = name.Value;
        UserAgent = userAgent.Value;
        TrustedUntil = trustedUntil;
        LastUsedDate = DateTime.UtcNow;
        IpAddress = ipAddress.Value;
    }

    //- - - - - - - - - - - - //

    // New overload accepting TrustDuration
    internal static TrustedDevice Create(
        AppUser user,
        DeviceFingerprint fingerprint,
        DeviceName name,
        UserAgent userAgent,
        IpAddress ipAddress,
        TrustDurationNullable trustDuration)
    {
        DateTime? trustedUntil = trustDuration.Value.HasValue
            ? DateTime.UtcNow.Add(trustDuration.Value!.Value)
            : null;

        var device = new TrustedDevice(
            user,
            fingerprint,
            name,
            userAgent,
            ipAddress,
            trustedUntil);

        device.RaiseDomainEvent(new TrustedDeviceAddedDomainEvent(device.Id, user.Id));

        return device;
    }

    //- - - - - - - - - - - - //

    public TrustedDevice UpdateLastUsed()
    {
        LastUsedDate = DateTime.UtcNow;
        RaiseDomainEvent(new TrustedDeviceUsedDomainEvent(Id, UserId));
        return this;
    }

    //- - - - - - - - - - - - //

    public bool IsExpired()
    {
        if (!TrustedUntil.HasValue) 
            return false;
        return TrustedUntil.Value < DateTime.UtcNow;

    }

    //- - - - - - - - - - - - //

    internal TrustedDevice Revoke()
    {
        TrustedUntil = DateTime.UtcNow;
        RaiseDomainEvent(new TrustedDeviceRevokedDomainEvent(Id, UserId));
        return this;
    }

    //- - - - - - - - - - - - //

    internal TrustedDevice ExtendTrust(TimeSpan? trustDuration)
    {
        TrustedUntil = trustDuration.HasValue
            ? DateTime.UtcNow.Add(trustDuration.Value)
            : null;

        RaiseDomainEvent(new TrustedDeviceExtendedDomainEvent(Id, UserId));
        return this;
    }

    //- - - - - - - - - - - - //

    #region EqualsAndHashCode
    public override bool Equals(object? obj) =>
        obj is TrustedDevice td
        && td.Fingerprint == Fingerprint
        && td.UserId == UserId;

    public override int GetHashCode() => HashCode.Combine(Fingerprint, UserId);
    #endregion

}
