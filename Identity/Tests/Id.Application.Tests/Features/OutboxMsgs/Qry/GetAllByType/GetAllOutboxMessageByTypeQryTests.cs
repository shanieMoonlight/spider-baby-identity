using ID.Application.Features.OutboxMessages.Qry.GetAllByType;
using ID.Application.Mediatr.CqrsAbs;
using Shouldly;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAllByType;

public class GetOutboxMessageByNameQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetAllOutboxMessagesByTypeQry("type");

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
