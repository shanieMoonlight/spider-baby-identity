using ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
using ID.Domain.Entities.AppUsers;
using Shouldly;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.AddCustomer;

public class AddCustomerMemberCmdTests
{
    [Fact]
    public void AddCustomerMemberCmd_Implements_IIdUserAndTeamAwareRequest()
    {
        // Arrange
        var dto = new AddCustomerMemberDto();
        var command = new AddCustomerMemberCmd(dto);


        // Act & Assert
        command.ShouldBeAssignableTo<IIdUserAndTeamAwareRequest<AppUser>>();
    }
}
