using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Features.MemberMgmt.Qry.GetMntcTeamMember;

internal class GetMntcTeamMemberQryHandler(IIdentityTeamManager<AppUser> _teamMgr) : IIdQueryHandler<GetMntcTeamMemberQry, AppUserDto>
{

    public async Task<GenResult<AppUserDto>> Handle(GetMntcTeamMemberQry request, CancellationToken cancellationToken)
    {
        var position = request.IsSuper ? 1000 : request.PrincipalTeamPosition;

        var member = (await _teamMgr.GetMntcTeamWithMemberAsync(request.MemberId, position))
                ?.Members
                ?.FirstOrDefault(u => u.Id == request.MemberId);

        return member == null
            ? GenResult<AppUserDto>.NotFoundResult(IDMsgs.Error.NotFound<AppUser>(request.MemberId))
            : GenResult<AppUserDto>.Success(member.ToDto());
    }

}//Cls

