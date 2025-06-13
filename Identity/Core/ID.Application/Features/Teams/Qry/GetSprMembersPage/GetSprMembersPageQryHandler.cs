using ID.Application.Dtos.User;
using MyResults;
using Pagination;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Members;

namespace ID.Application.Features.Teams.Qry.GetSprMembersPage;
internal class GetSprMembersPageQryHandler(IIdentityMemberAuditService<AppUser> teamRepo)
    : IIdQueryHandler<GetSprMembersPageQry, PagedResponse<AppUserDto>>
{
    public async Task<GenResult<PagedResponse<AppUserDto>>> Handle(GetSprMembersPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest;

        var page = await teamRepo.GetSuperPageAsync(pgRequest, request.PrincipalTeamPosition);
        var pageResponse = new PagedResponse<AppUser>(page, pgRequest);

        return GenResult<PagedResponse<AppUserDto>>.Success(pageResponse.Transform(m => m.ToDto()));
    }

}//Cls
