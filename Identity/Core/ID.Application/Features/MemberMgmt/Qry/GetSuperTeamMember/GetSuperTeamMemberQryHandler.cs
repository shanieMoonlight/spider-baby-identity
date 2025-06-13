using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Qry.GetSuperTeamMember;

internal class GetSuperTeamMemberQryHandler() : IIdQueryHandler<GetSuperTeamMemberQry, AppUserDto>
{

    public Task<GenResult<AppUserDto>> Handle(GetSuperTeamMemberQry request, CancellationToken cancellationToken)
    {
        //Only Super members can get here.
        var member = request.PrincipalTeam.Members
             .FirstOrDefault(u => u.Id == request.MemberId);

        return member == null
            ? Task.FromResult(GenResult<AppUserDto>.NotFoundResult(IDMsgs.Error.NotFound<AppUser>(request.MemberId)))
            : Task.FromResult(GenResult<AppUserDto>.Success(member.ToDto()));
    }

}//Cls

