using Shouldly;
using ID.Application.Features.OutboxMessages.Qry.GetAll;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAll;

public class GetAllOutboxMessagesQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetAllOutboxMessagesQry();

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
