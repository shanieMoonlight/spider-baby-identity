using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ID.Domain.Entities.AppUsers.OAuth;

public class OAuthInfo : IdDomainEntity
{
    public string? Issuer { get; set; }

    public string? ImageUrl { get; set; }

    public bool? EmailVerified { get; set; }

    [Column(TypeName = "nvarchar(40)")]
    public OAuthProvider Provider { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; }


    //------------------------//

    #region EfCoreCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public OAuthInfo() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    //- - - - - - - - - - - - //

    public static OAuthInfo Create(
        OAuthProvider provider,
        IssuerNullable issuer,
        ImgUrlNullable imgUrl,
        EmailVerifiedNullable emailVerified)
    {
        var oAuth = new OAuthInfo()
        {
            Provider = provider,
            Issuer = issuer.Value,
            ImageUrl = imgUrl.Value,
            EmailVerified = emailVerified.Value
        };

        return oAuth;

    }


    //------------------------//
}
