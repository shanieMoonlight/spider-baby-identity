using Shouldly;
using ID.Application.Features.OutboxMessages.Qry.GetById;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetById;

public class GetOutboxMessageByIdQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetOutboxMessageByIdQry(Guid.NewGuid());

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
