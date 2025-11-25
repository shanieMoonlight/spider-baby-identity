using ID.Domain.Entities.Refreshing;

namespace ID.Application.Features.IdRefreshTokens;

public static class IdRefreshTokenMappings
{
    public static IdRefreshTokenDto ToDto(this IdRefreshToken mdl) => new(mdl);
} //Cls
