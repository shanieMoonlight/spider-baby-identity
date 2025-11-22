using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers.TrustedDevices;

public class TrustedDeviceTests
{
    [Fact]
    public void Create_WithTrustDuration_ShouldSetTrustedUntil()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var fp = DeviceFingerprint.Create($"fp-{Guid.NewGuid()}");
        var name = DeviceName.Create("Device A");
        var ua = UserAgent.Create("UA-A");
        var duration = TrustDurationNullable.Create(TimeSpan.FromDays(7));

        // Act
        var device = TrustedDevice.Create(user, fp, name, ua, duration);

        // Assert
        device.UserId.ShouldBe(user.Id);
        device.DeviceFingerprint.ShouldBe(fp.Value);
        device.Name.ShouldBe(name.Value);
        device.UserAgent.ShouldBe(ua.Value);
        device.TrustedUntil.ShouldNotBeNull();
        // TrustedUntil should be approximately now + duration
        var diff = device.TrustedUntil!.Value - DateTime.UtcNow - duration.Value!.Value;
        Math.Abs(diff.TotalSeconds).ShouldBeLessThan(2);
    }

    //--------------------//

    [Fact]
    public void Create_WithNullTrustDuration_ShouldBeIndefinite()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var fp = DeviceFingerprint.Create($"fp-{Guid.NewGuid()}");
        var name = DeviceName.Create("Device B");
        var ua = UserAgent.Create("UA-B");
        var duration = TrustDurationNullable.Create(null);

        // Act
        var device = TrustedDevice.Create(user, fp, name, ua, duration);

        // Assert
        device.TrustedUntil.ShouldBeNull();
        device.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void UpdateLastUsed_ShouldChangeLastUsedDate()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("D"), UserAgent.Create("UA"), TrustDurationNullable.Create(null));

        var before = device.LastUsedDate;
        Thread.Sleep(5);

        // Act
        device.UpdateLastUsed();

        // Assert
        device.LastUsedDate.ShouldBeGreaterThan(before);
    }

    //--------------------//

    [Fact]
    public void IsExpired_ReturnsTrueForPastTrustedUntilAndFalseOtherwise()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        // expired device
        var expiredDevice = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("E"), UserAgent.Create("UA"), TrustDurationNullable.Create(TimeSpan.FromSeconds(-10)));
        // active device
        var activeDevice = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("A"), UserAgent.Create("UA"), TrustDurationNullable.Create(TimeSpan.FromHours(1)));
        // indefinite device
        var indefiniteDevice = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("I"), UserAgent.Create("UA"), TrustDurationNullable.Create(null));

        // Act & Assert
        expiredDevice.IsExpired().ShouldBeTrue();
        activeDevice.IsExpired().ShouldBeFalse();
        indefiniteDevice.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void Revoke_ShouldSetTrustedUntilToNowAndMarkExpired()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("R"), UserAgent.Create("UA"), TrustDurationNullable.Create(TimeSpan.FromDays(1)));

        // Act
        var revoked = device.Revoke();

        // Assert
        revoked.TrustedUntil.ShouldNotBeNull();
        // TrustedUntil set to now (approximately)
        var diff = DateTime.UtcNow - revoked.TrustedUntil!.Value;
        Math.Abs(diff.TotalSeconds).ShouldBeLessThan(2);
        revoked.IsExpired().ShouldBeTrue();
    }

    //--------------------//

    [Fact]
    public void ExtendTrust_WithDuration_ShouldSetTrustedUntil()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("X"), UserAgent.Create("UA"), TrustDurationNullable.Create(TimeSpan.FromDays(1)));

        // Act
        var extended = device.ExtendTrust(TimeSpan.FromDays(5));

        // Assert
        extended.TrustedUntil.ShouldNotBeNull();
        extended.TrustedUntil!.Value.ShouldBeGreaterThan(DateTime.UtcNow);
        extended.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void ExtendTrust_WithNull_ShouldMakeIndefinite()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("Y"), UserAgent.Create("UA"), TrustDurationNullable.Create(TimeSpan.FromDays(1)));

        // Act
        var extended = device.ExtendTrust(null);

        // Assert
        extended.TrustedUntil.ShouldBeNull();
        extended.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void EqualsAndGetHashCode_DependsOnFingerprintAndUserId()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var fpValue = $"same-fp-{Guid.NewGuid()}";

        var d1 = TrustedDevice.Create(user, DeviceFingerprint.Create(fpValue), DeviceName.Create("N1"), UserAgent.Create("UA1"), TrustDurationNullable.Create(null));
        var d2 = TrustedDevice.Create(user, DeviceFingerprint.Create(fpValue), DeviceName.Create("N2"), UserAgent.Create("UA2"), TrustDurationNullable.Create(TimeSpan.FromDays(1)));

        // Act & Assert
        d1.Equals(d2).ShouldBeTrue();
        d1.GetHashCode().ShouldBe(d2.GetHashCode());
    }
}
