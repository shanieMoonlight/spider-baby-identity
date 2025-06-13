using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.MemberMgmt.Qry.GetMyTeamMember;

internal class GetMyTeamMemberQryHandler() : IIdQueryHandler<GetMyTeamMemberQry, AppUserDto>
{

    public Task<GenResult<AppUserDto>> Handle(GetMyTeamMemberQry request, CancellationToken cancellationToken)
    {
        var member = request
            .PrincipalTeam
            .Members
            .FirstOrDefault(m => m.Id == request.MemberId);

        if (member is null)
            return Task.FromResult(GenResult<AppUserDto>.NotFoundResult(IDMsgs.Error.NotFound<AppUser>(request.MemberId)));

        return Task.FromResult(GenResult<AppUserDto>.Success(member.ToDto()));
    }

}//Cls

