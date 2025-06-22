namespace ID.Application.MFA;


public interface ITwofactorUserIdCacheService
{
    public string StoreUserId(Guid userId);

    public Guid? GetUserId(string token);

    public void RemoveToken(string token);

}//Int
