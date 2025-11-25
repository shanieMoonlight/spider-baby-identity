using ID.Application.AppImps.TrustedDevices;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ID.Application.Tests.AppImps.TrustedDevices;

public class DeviceTrustServiceTests
{
    [Fact]
    public async Task TrustAsync_ReturnsSuccess_WhenTrustedDeviceServiceSucceeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var device = TrustedDeviceDataFactory.Create(user: user);

        var trustedSvcMock = new Mock<ITrustedDeviceService<AppUser>>();
        trustedSvcMock
            .Setup(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<IpAddress>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.Success(device));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.UserAgent = "UnitTestAgent";
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("192.168.0.10");

        var httpAccessorMock = new Mock<IHttpContextAccessor>();
        httpAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext);

        var svc = new DeviceTrustService<AppUser>(trustedSvcMock.Object, httpAccessorMock.Object);

        // Act
        var result = await svc.TrustAsync(user, "fingerprint-123", "MyDevice");

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(device.Id);

        trustedSvcMock.Verify(s => s.AddAsync(
            It.Is<AppUser>(u => u.Id == user.Id),
            It.Is<DeviceFingerprint>(fp => fp.Value == "fingerprint-123"),
            It.Is<DeviceName>(n => n.Value == "MyDevice"),
            It.Is<UserAgent>(ua => ua.Value == "UnitTestAgent"),
            It.Is<IpAddress>(ip => ip.Value == "192.168.0.10"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task TrustAsync_ReturnsFailure_WhenTrustedDeviceServiceFails()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        var trustedSvcMock = new Mock<ITrustedDeviceService<AppUser>>();
        trustedSvcMock
            .Setup(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<IpAddress>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.BadRequestResult("bad"));

        var httpAccessorMock = new Mock<IHttpContextAccessor>();
        httpAccessorMock.SetupGet(x => x.HttpContext).Returns((HttpContext?)null);

        var svc = new DeviceTrustService<AppUser>(trustedSvcMock.Object, httpAccessorMock.Object);

        // Act
        var result = await svc.TrustAsync(user, "fp", "name");

        // Assert
        result.Succeeded.ShouldBeFalse();
    }

    //--------------------//

    [Fact]
    public async Task TrustAsync_UsesDefaults_When_HttpContextMissing()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        DeviceFingerprint capturedFp = null!;
        UserAgent capturedUa = null!;
        IpAddress capturedIp = null!;

        var trustedSvcMock = new Mock<ITrustedDeviceService<AppUser>>();
        trustedSvcMock
            .Setup(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<IpAddress>(),
                It.IsAny<CancellationToken>()))
            .Callback<AppUser, DeviceFingerprint, DeviceName, UserAgent, IpAddress, CancellationToken>((u, fp, nm, ua, ip, ct) =>
            {
                capturedFp = fp;
                capturedUa = ua;
                capturedIp = ip;
            })
            .ReturnsAsync(GenResult<TrustedDevice>.Success(TrustedDeviceDataFactory.Create(user: user)));

        var httpAccessorMock = new Mock<IHttpContextAccessor>();
        httpAccessorMock.SetupGet(x => x.HttpContext).Returns((HttpContext?)null);

        var svc = new DeviceTrustService<AppUser>(trustedSvcMock.Object, httpAccessorMock.Object);

        // Act
        var result = await svc.TrustAsync(user, "fp-default", "dev-name");

        // Assert
        result.Succeeded.ShouldBeTrue();
        capturedUa.ShouldNotBeNull();
        capturedUa.Value.ShouldBe("Unknown UserAgent");
        capturedIp.ShouldNotBeNull();
        capturedIp.Value.ShouldBe("Unknown IP Address");
        capturedFp.Value.ShouldBe("fp-default");
    }
}
