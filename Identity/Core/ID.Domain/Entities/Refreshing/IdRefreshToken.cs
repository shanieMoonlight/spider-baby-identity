using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.Refreshing.ValueObjects;
using MassTransit;


namespace ID.Domain.Entities.Refreshing;
public class IdRefreshToken : IdDomainEntity
{

    public string Payload { get; set; } = string.Empty;
    public DateTime ExpiresOnUtc { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    //FKs
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }

    //- - - - - - - - - - - - //   

    public bool IsExpired { get => ExpiresOnUtc < DateTime.UtcNow; }

    //------------------------//   

    #region EfCoreCtor
    protected IdRefreshToken() { }
    #endregion

    private IdRefreshToken(TokenPayload token, AppUser user, TokenLifetime tokenLifetime)
        : base(NewId.NextSequentialGuid())
    {
        Payload = token.Value;
        ExpiresOnUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
        UserId = user.Id;
        User = user;
    }

    //------------------------//   

    public static IdRefreshToken Create(TokenPayload token,  AppUser user, TokenLifetime tokenLifetime) => 
        new(token, user, tokenLifetime);

    //------------------------//    

    public IdRefreshToken Update(TokenPayload token, TokenLifetime tokenLifetime)
    {
        Payload = token.Value;
        ExpiresOnUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
        return this;
    }

}//Cls
