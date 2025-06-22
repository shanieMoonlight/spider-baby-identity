using ID.Application.MFA;
using ID.Infrastructure.Utility;
using Microsoft.Extensions.Caching.Memory;

namespace ID.Infrastructure.Services.MFA;
internal class TwofactorUserIdCacheService(IMemoryCache _cache) : ITwofactorUserIdCacheService
{
    public string StoreUserId(Guid userId)
    {
        var token = RandomTokenGenerator.Generate();
        _cache.Set(token, userId, TimeSpan.FromMinutes(10));
        return token;
    }

    public Guid? GetUserId(string token)
        => _cache.TryGetValue(token, out Guid userId) ? userId : null;

    public void RemoveToken(string token)
        => _cache.Remove(token);
}