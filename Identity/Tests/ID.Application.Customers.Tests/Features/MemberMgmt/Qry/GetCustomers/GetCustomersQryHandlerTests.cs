using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomers;
using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Qry.GetCustomers;

public class GetCustomersQryHandlerTests
{
    private readonly Mock<IIdentityMemberAuditService<AppUser>> _mockCustomerAuditService;
    private readonly GetCustomersQryHandler _handler;

    public GetCustomersQryHandlerTests()
    {
        _mockCustomerAuditService = new Mock<IIdentityMemberAuditService<AppUser>>();
        _handler = new GetCustomersQryHandler(_mockCustomerAuditService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnListOfAppUserDto()
    {
        // Arrange
        var customers = AppUserDataFactory.CreateMany(2);

        _mockCustomerAuditService.Setup(service => service.GetCustomersAsync()).ReturnsAsync(customers);

        var request = new GetCustomersQry();
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldNotBeNull();
        result.Value.Count().ShouldBe(2);
        result.Value.First().UserName.ShouldBe(customers.First().UserName);
        result.Value.Last().UserName.ShouldBe(customers.Last().UserName);
    }

    //------------------------------------//

}//Cls