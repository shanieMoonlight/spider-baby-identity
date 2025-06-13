using ID.Application.Features.Common.Dtos.User;
using ID.Application.Dtos.User;
using MyResults;
using Pagination;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Members;

namespace ID.Application.Features.Teams.Qry.GetMntcMembersPage;
internal class GetMntcMembersPageQryHandler(IIdentityMemberAuditService<AppUser> _mbrService)
    : IIdQueryHandler<GetMntcMembersPageQry, PagedResponse<AppUserDto>>
{
    public async Task<GenResult<PagedResponse<AppUserDto>>> Handle(GetMntcMembersPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest;

        //Super Members see all Mntc Members, Mntc Members see only their Position and lower
        var teamPosition = request.IsSuperMinimum ? 10000 : request.PrincipalTeamPosition;
        var page = await _mbrService.GetMntcPageAsync(pgRequest, teamPosition);
        var pageResponse = new PagedResponse<AppUser>(page, pgRequest);

        return GenResult<PagedResponse<AppUserDto>>.Success(pageResponse.Transform(m => m.ToDto()));
    }


}//Cls
