namespace ID.Application.Features.IdRefreshTokens.Qry.GetPage;
public record GetIdRefreshTokensPageQry(PagedRequest? PagedRequest) :AIdQuery<PagedResponse<IdRefreshTokenDto>>;