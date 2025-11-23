using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Entities.TrustedDevices.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdHandlerTests
{
    [Fact]
    public async Task Should_Call_AddAsync_And_Return_Success_When_Service_Succeeds()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var userAgent = "ua-1";

        var dto = new TrustDeviceCreateDto(
            DeviceFingerprint: "fp-123",
            DeviceName: "My Device");

        var cmd = new TrustDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var trustedDevice = TrustedDeviceDataFactory.Create(
            user: user,
            deviceFingerprint: dto.DeviceFingerprint,
            name: dto.DeviceName,
            userAgent: userAgent
        );

        var mockService = new Mock<Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.AddAsync(
                It.Is<AppUser>(u => u == user),
                It.Is<DeviceFingerprint>(df => df.Value == dto.DeviceFingerprint),
                It.Is<DeviceName>(dn => dn.Value == dto.DeviceName),
                It.Is<UserAgent>(ua => ua.Value == userAgent),
                It.IsAny<IpAddress>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.Success(trustedDevice));

        // Setup HttpContextAccessor with headers and remote IP
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.UserAgent = userAgent;
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        var handler = new TrustDeviceCmdHandler(mockService.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<IpAddress>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.DeviceFingerprint.ShouldBe(trustedDevice.Fingerprint);
    }

    //--------------------------// 

    [Fact]
    public async Task Should_Return_BadRequest_When_Service_Returns_BadRequest()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        var dto = new TrustDeviceCreateDto(
            DeviceFingerprint: "fp-456",
            DeviceName: "Other Device"
        );

        var cmd = new TrustDeviceCmd(dto)
        {
            PrincipalUser = user
        };

        var errorMsg = "invalid request";

        var mockService = new Mock<Domain.Abstractions.Services.TrustedDevices.ITrustedDeviceService<AppUser>>();
        mockService.Setup(s => s.AddAsync(
                It.IsAny<AppUser>(),
                It.IsAny<DeviceFingerprint>(),
                It.IsAny<DeviceName>(),
                It.IsAny<UserAgent>(),
                It.IsAny<IpAddress>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.BadRequestResult(errorMsg));

        // Setup HttpContextAccessor to avoid null reference
        var httpContext = new DefaultHttpContext();
        // empty user-agent and no remote ip
        httpContext.Request.Headers.UserAgent = string.Empty;
        httpContext.Connection.RemoteIpAddress = null;
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        var handler = new TrustDeviceCmdHandler(mockService.Object, httpContextAccessorMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(errorMsg);
    }
}
