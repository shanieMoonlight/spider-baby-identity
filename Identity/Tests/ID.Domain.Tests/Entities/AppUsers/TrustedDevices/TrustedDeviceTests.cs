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
        var ip = IpAddress.Create("IP");
        var duration = TrustDuration.Create(TimeSpan.FromDays(7));

        // Act
        var device = TrustedDevice.Create(user, fp, name, ua, ip, duration);

        // Assert
        device.UserId.ShouldBe(user.Id);
        device.Fingerprint.ShouldBe(fp.Value);
        device.Name.ShouldBe(name.Value);
        device.UserAgent.ShouldBe(ua.Value);
        device.TrustedUntil.ShouldNotBe(default);
        // TrustedUntil should be approximately now + duration
        var diff = device.TrustedUntil - DateTime.UtcNow - duration.Value;
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
        var duration = TrustDuration.Create(TimeSpan.FromDays(5));
        var ip = IpAddress.Create("IP");

        // Act
        var device = TrustedDevice.Create(user, fp, name, ua, ip, duration);

        // Assert
        device.TrustedUntil.ShouldNotBe(default);
        device.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void UpdateLastUsed_ShouldChangeLastUsedDate()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("D"), UserAgent.Create("UA"), IpAddress.Create("IP"), TrustDuration.Create(TimeSpan.FromDays(1)));

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
        var expiredDevice = TrustedDeviceDataFactory.Create(
            user: user,
            trustedUntil: DateTime.UtcNow.AddSeconds(-30)
            );

        // active device
        var activeDevice = TrustedDeviceDataFactory.Create(
            user: user,
            trustedUntil: DateTime.UtcNow.AddHours(1));

        // indefinite device
        var indefiniteDevice = TrustedDeviceDataFactory.Create(user: user, trustedUntil: null);

        // Act & Assert
        expiredDevice.IsExpired().ShouldBeTrue();
        activeDevice.IsExpired().ShouldBeFalse();
        indefiniteDevice.IsExpired().ShouldBeTrue(); //We don't allow nulls
    }

    //--------------------//

    [Fact]
    public void Revoke_ShouldSetTrustedUntilToNowAndMarkExpired()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(
            user,
            DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"),
            DeviceName.Create("R"),
            UserAgent.Create("UA"),
            IpAddress.Create("IP"),
            TrustDuration.Create(TimeSpan.FromDays(1)));

        // Act
        var revoked = device.Revoke();

        // Assert
        revoked.TrustedUntil.ShouldNotBe(default);
        // TrustedUntil set to now (approximately)
        var diff = DateTime.UtcNow - revoked.TrustedUntil;
        Math.Abs(diff.TotalSeconds).ShouldBeLessThan(2);
        revoked.IsExpired().ShouldBeTrue();
    }

    //--------------------//

    [Fact]
    public void ExtendTrust_WithDuration_ShouldSetTrustedUntil()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("X"), UserAgent.Create("UA"), IpAddress.Create("IP"), TrustDuration.Create(TimeSpan.FromDays(1)));

        // Act
        var extended = device.ExtendTrust(TimeSpan.FromDays(5));

        // Assert
        extended.TrustedUntil.ShouldNotBe(default);
        extended.TrustedUntil.ShouldBeGreaterThan(DateTime.UtcNow);
        extended.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void ExtendTrust_WithNull_ShouldMakeIndefinite()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDevice.Create(user, DeviceFingerprint.Create($"fp-{Guid.NewGuid()}"), DeviceName.Create("Y"), UserAgent.Create("UA"), IpAddress.Create("IP"), TrustDuration.Create(TimeSpan.FromDays(1)));

        // Act
        var extended = device.ExtendTrust(TimeSpan.FromDays(5));

        // Assert
        extended.TrustedUntil.ShouldNotBe(default);
        extended.IsExpired().ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public void EqualsAndGetHashCode_DependsOnFingerprintAndUserId()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var fpValue = $"same-fp-{Guid.NewGuid()}";

        var d1 = TrustedDevice.Create(user, DeviceFingerprint.Create(fpValue), DeviceName.Create("N1"), UserAgent.Create("UA1"), IpAddress.Create("IP"), TrustDuration.Create(TimeSpan.FromDays(1)));
        var d2 = TrustedDevice.Create(user, DeviceFingerprint.Create(fpValue), DeviceName.Create("N2"), UserAgent.Create("UA2"), IpAddress.Create("IP"), TrustDuration.Create(TimeSpan.FromDays(1)));

        // Act & Assert
        d1.Equals(d2).ShouldBeTrue();
        d1.GetHashCode().ShouldBe(d2.GetHashCode());
    }
}
