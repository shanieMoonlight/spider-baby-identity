using ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
using ID.Application.Mediatr.CqrsAbs;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.AddCustomer_Mntc;

public class AddCustomerMember_MntcCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IIdUserAndTeamAwareRequest()
    {
        // Arrange
        var dto = new AddCustomerMember_MntcDto();
        var command = new AddCustomerMemberCmd_Mntc(dto);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
