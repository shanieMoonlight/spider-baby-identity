namespace ID.Application.Features.IdRefreshTokens.Qry.GetById;
public record GetIdRefreshTokenByIdQry(Guid? Id) : AIdQuery<IdRefreshTokenDto>;