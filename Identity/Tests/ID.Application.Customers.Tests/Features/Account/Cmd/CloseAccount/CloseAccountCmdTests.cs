//using Shouldly;
//using ID.Domain.Features.Account.Cmd.Customer.AddCustomerMember;
//using ID.Application.Mediatr.CqrsAbstractions;
//using ID.Domain.Entities.AppUsers;

//namespace ID.Domain.Tests.Features.Account.Cmd.CloseAccount;

//public class CloseAccountCmdTests
//{
//    [Fact]
//    public void CloseAccountCmdTests_Implements_IIdUserAndTeamAwareRequest()
//    {
//        // Arrange
//        var dto = new AddCustomerMemberDto();
//        var command = new AddCustomerMemberCmd(dto);

//        // Act & Assert
//        Assert.IsAssignableFrom<IIdUserAndTeamAwareRequest<AppUser>>(command);

//        // Act & Assert
//        command.ShouldBeAssignableTo<IIdUserAndTeamAwareRequest<AppUser>>();
//    }
//}
