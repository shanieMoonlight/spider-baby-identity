using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomersPage;
using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using Pagination;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Qry.GetCustomersPage;

public class GetCustomersPageQryHandlerTests
{
    private readonly Mock<IIdentityMemberAuditService<AppUser>> _mockCustomerAuditService;
    private readonly GetCustomersPageQryHandler _handler;

    public GetCustomersPageQryHandlerTests()
    {
        _mockCustomerAuditService = new Mock<IIdentityMemberAuditService<AppUser>>();
        _handler = new GetCustomersPageQryHandler(_mockCustomerAuditService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnListOfAppUserDto()
    {
        // Arrange
        var customers = AppUserDataFactory.CreateMany(2);

        var request = new GetCustomersPageQry(null);
        var page = new Page<AppUser>(customers, 1, 1);

        _mockCustomerAuditService.Setup(service => service.GetCustomerPageAsync(It.IsAny<PagedRequest>()))
            .ReturnsAsync(page);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeAssignableTo<PagedResponse<AppUser_Customer_Dto>>();
    }

    //------------------------------------//

}//Cls