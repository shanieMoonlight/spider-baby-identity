using ID.Domain.Repos;

namespace ID.Application.Features.IdRefreshTokens.Qry.GetPage;
internal class GetIdRefreshTokensPageQryHandler(IIdentityRefreshTokenRepo repo) : IIdQueryHandler<GetIdRefreshTokensPageQry, PagedResponse<IdRefreshTokenDto>>
{

    public async Task<GenResult<PagedResponse<IdRefreshTokenDto>>> Handle(GetIdRefreshTokensPageQry request, CancellationToken cancellationToken)
    {
        var pgRequest = request.PagedRequest ?? PagedRequest.Empty();

        var page = (await repo.PageAsync(pgRequest.PageNumber, pgRequest.PageSize, pgRequest.SortList, pgRequest.FilterList))
                   .Transform((d) => d.ToDto());

        var response = new PagedResponse<IdRefreshTokenDto>(page, pgRequest);

        return GenResult<PagedResponse<IdRefreshTokenDto>>.Success(response);

    }//Handle


}//Cls
