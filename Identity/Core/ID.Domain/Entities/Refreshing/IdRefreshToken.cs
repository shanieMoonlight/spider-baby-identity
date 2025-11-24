using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.Domain.Entities.TrustedDevices;
using MassTransit;


namespace ID.Domain.Entities.Refreshing;
public class IdRefreshToken : IdDomainEntity
{

    public string Payload { get; set; } = string.Empty;
    public DateTime ExpiresOnUtc { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public List<AuthMethodRef> AuthMethodRefs { get; private set; } = [];

    //FKs
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }

    public TrustedDevice? TrustedDevice { get; set; }
    public Guid? TrustedDeviceId { get; set; }
       

    //- - - - - - - - - - - - //   

    public bool IsExpired { get => ExpiresOnUtc < DateTime.UtcNow; }

    //------------------------//   

    #region EfCoreCtor
    protected IdRefreshToken() { }
    #endregion

    private IdRefreshToken(TokenPayload token, AppUser user, TokenLifetime tokenLifetime, IEnumerable<AuthMethodRef> authMethodRefs )
        : base(NewId.NextSequentialGuid())
    {
        Payload = token.Value;
        ExpiresOnUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
        UserId = user.Id;
        User = user;
        AuthMethodRefs = [.. authMethodRefs];
    }

    //------------------------//   

    public static IdRefreshToken Create(TokenPayload token,  AppUser user, TokenLifetime tokenLifetime, IEnumerable<AuthMethodRef> authMethodRefs) => 
        new(token, user, tokenLifetime, authMethodRefs);

    //------------------------//    

    public IdRefreshToken Update(TokenPayload token, TokenLifetime tokenLifetime)
    {
        Payload = token.Value;
        ExpiresOnUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
        return this;
    }

}//Cls
