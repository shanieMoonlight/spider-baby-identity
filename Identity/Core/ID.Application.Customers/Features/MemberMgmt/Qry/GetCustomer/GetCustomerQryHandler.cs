using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomer;

internal class GetCustomerQryHandler(IIdentityTeamManager<AppUser> teamMgr)
    : IIdQueryHandler<GetCustomerQry, AppUser_Customer_Dto>
{

    public async Task<GenResult<AppUser_Customer_Dto>> Handle(GetCustomerQry request, CancellationToken cancellationToken)
    {
        var userId = request.MemberId;
        var teamId = request.TeamId;

        var team = await teamMgr.GetByIdWithMemberAsync(teamId, userId);

        var dbCustomer = team?.Members?.FirstOrDefault(u => u.Id == userId);
        if (dbCustomer is null)
            return GenResult<AppUser_Customer_Dto>.NotFoundResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(userId, team?.Name ?? teamId.ToString()));

        return GenResult<AppUser_Customer_Dto>.Success(dbCustomer.ToCustomerDto());
    }

}//Cls

