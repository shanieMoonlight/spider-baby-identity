namespace ID.Application.JWT;

public interface IJsonWebKeyProvider
{
    //JwksDto CreateJwks(string pem);
    Task<JwkListDto> GetJwks();
}