using ID.Domain.Claims.AuthMethods;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.Domain.Entities.TrustedDevices;
using MassTransit;


namespace ID.Domain.Entities.Refreshing;
public class IdRefreshToken : IdDomainEntity
{

    public string Selector { get; set; } = string.Empty;
    public string PayloadHash { get; set; } = string.Empty;
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

    private IdRefreshToken(
        TokenPayloadHash token,
        TokenSelector selector,
        AppUser user, 
        TokenLifetime tokenLifetime, 
        IEnumerable<AuthMethodRef> authMethodRefs )
        : base(NewId.NextSequentialGuid())
    {
        PayloadHash = token.Value;
        ExpiresOnUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
        UserId = user.Id;
        User = user;
        Selector = selector.Value;
        AuthMethodRefs = [.. authMethodRefs];
    }

    //------------------------//   

    public static IdRefreshToken Create(
        TokenPayloadHash payload,
        TokenSelector selector,
        AppUser user,
        TokenLifetime tokenLifetime,
        IEnumerable<AuthMethodRef> authMethodRefs) =>
        new(payload, selector, user, tokenLifetime, authMethodRefs);

    //------------------------//   

    public static IdRefreshToken Create(
        TokenPayloadHash token, 
        TokenSelector selector,
        AppUser user,
        TokenLifetime tokenLifetime, 
        IEnumerable<AuthMethodRef> authMethodRefs,
        TrustedDevice trustedDevice) =>
        new(token, selector, user, tokenLifetime, authMethodRefs)
        {
            TrustedDevice = trustedDevice
        };

    //------------------------//    

    public IdRefreshToken Update(TokenPayloadHash token, TokenLifetime tokenLifetime)
    {
        PayloadHash = token.Value;
        ExpiresOnUtc = DateTime.UtcNow.Add(tokenLifetime.Value);
        return this;
    }

}//Cls
