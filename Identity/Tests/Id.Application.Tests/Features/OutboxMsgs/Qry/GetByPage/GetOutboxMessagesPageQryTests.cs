using Shouldly;
using ID.Application.Features.OutboxMessages.Qry.GetPage;
using Pagination;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetByPage;

public class GetByNameQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetOutboxMessagePageQry(PagedRequest.Empty());

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
