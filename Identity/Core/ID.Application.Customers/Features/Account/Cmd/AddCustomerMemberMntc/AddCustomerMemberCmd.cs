using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;

/// <summary>
/// 
/// For use by Mntc Team to add a Customer to an existing Cusotmer Team.
/// Might be required if something goes wrong with a customer account.
/// Must be Mntc or Super to use
/// </summary>
/// <param name="Dto"></param>
public record AddCustomerMemberCmd_Mntc(AddCustomerMember_MntcDto Dto)
    : AIdCommand<AppUser_Customer_Dto>;



