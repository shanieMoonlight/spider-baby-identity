using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetPage;
using ID.Domain.Entities.TrustedDevices;
using Pagination;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetPage;

public class GetTrustedDevicesPageQryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Paged_Response_With_Devices()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var devices = new HashSet<TrustedDevice>();
        for (int i = 0; i < 5; i++)
        {
            devices.Add(TrustedDeviceDataFactory.Create(userId: userId));
        }

        var pageNumber = 1;
        var pageSize = 2;

        var user = AppUserDataFactory.Create(id: userId, trustedDevices: devices);

        var pgRequest = new PagedRequest(pageNumber: pageNumber, pageSize: pageSize);
        var qry = new GetTrustedDevicesPageQry(pgRequest)
        {
            PrincipalUser = user
        };

        var handler = new GetTrustedDevicesPageQryHandler();

        // Act
        var result = await handler.Handle(qry, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value?.Data.Count().ShouldBe(pageSize);
        result.Value?.PageNumber.ShouldBe(pageNumber);
        result.Value?.PageSize.ShouldBe(pageSize);
    }
}
