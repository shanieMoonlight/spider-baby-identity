using Google.Apis.Auth;

namespace ID.OAuth.Google.Data;
public class GoogleVerifiedPayload(GoogleJsonWebSignature.Payload googlePayload)
{
    /// <summary>
    /// A space-delimited list of the permissions the application requests or <c>null</c>.
    /// </summary>
    public string Scope { get; set; } = googlePayload.Scope ?? string.Empty;

    /// <summary>
    /// The email address of the user for which the application is requesting delegated access.
    /// </summary>
    public string Prn { get; set; } = googlePayload.Prn ?? string.Empty;

    /// <summary>
    /// The hosted GSuite domain of the user. Provided only if the user belongs to a hosted domain.
    /// </summary>
    public string HostedDomain { get; set; } = googlePayload.HostedDomain ?? string.Empty;

    /// <summary>
    /// The user's email address. This may not be unique and is not suitable for use as a primary key.
    /// Provided only if your scope included the string "email".
    /// </summary>
    public string Email { get; set; } = googlePayload.Email ?? string.Empty;

    /// <summary>
    /// True if the user's e-mail address has been verified; otherwise false.
    /// </summary>
    public bool EmailVerified { get; set; } = googlePayload.EmailVerified;

    /// <summary>
    /// The user's full name, in a displayable form. Might be provided when:
    /// (1) The request scope included the string "profile"; or
    /// (2) The ID token is returned from a token refresh.
    /// When name claims are present, you can use them to update your app's user records.
    /// Note that this claim is never guaranteed to be present.
    /// </summary>
    public string Name { get; set; } = googlePayload.Name ?? string.Empty;

    /// <summary>
    /// Given name(s) or first name(s) of the End-User. Note that in some cultures, people can have multiple given names;
    /// all can be present, with the names being separated by space characters.
    /// </summary>
    public string GivenName { get; set; } = googlePayload.GivenName ?? string.Empty;

    /// <summary>
    /// Surname(s) or last name(s) of the End-User. Note that in some cultures,
    /// people can have multiple family names or no family name;
    /// all can be present, with the names being separated by space characters.
    /// </summary>
    public string FamilyName { get; set; } = googlePayload.FamilyName ?? string.Empty;

    /// <summary>
    /// The URL of the user's profile picture. Might be provided when:
    /// (1) The request scope included the string "profile"; or
    /// (2) The ID token is returned from a token refresh.
    /// When picture claims are present, you can use them to update your app's user records.
    /// Note that this claim is never guaranteed to be present.
    /// </summary>
    public string Picture { get; set; } = googlePayload.Picture ?? string.Empty;

    /// <summary>
    /// End-User's locale, represented as a BCP47 [RFC5646] language tag.
    /// This is typically an ISO 639-1 Alpha-2 [ISO639‑1] language code in lowercase and an
    /// ISO 3166-1 Alpha-2 [ISO3166‑1] country code in uppercase, separated by a dash.
    /// For example, en-US or fr-CA.
    /// </summary>
    public string Locale { get; set; } = googlePayload.Locale ?? string.Empty;

    /// <summary>
    /// Issuer claim that identifies the principal that issued the JWT or <c>null</c>.
    /// </summary>
    public string Issuer { get; set; } = googlePayload.Issuer ?? string.Empty;

    /// <summary>
    /// Subject claim identifying the principal that is the subject of the JWT or <c>null</c>.
    /// </summary>
    public string Subject { get; set; } = googlePayload.Subject ?? string.Empty;

    /// <summary>
    /// Audience claim that identifies the audience that the JWT is intended for (should either be
    /// a string or list) or <c>null</c>.
    /// </summary>
    public object Audience { get; set; } = googlePayload.Audience ?? string.Empty;

    /// <summary>
    /// The target audience claim that identifies the audience that an OIDC token generated from
    /// this JWT is intended for. Maybe be null. Multiple target audiences are not supported.
    /// <c>null</c>.
    /// </summary>
    public string TargetAudience { get; set; } = googlePayload.TargetAudience ?? string.Empty;

    /// <summary>
    /// Gets or sets expiration time claim that identifies the expiration time (in seconds) on or after which 
    /// the token MUST NOT be accepted for processing or <c>null</c>.
    /// </summary>
    public long? ExpirationTimeSeconds { get; set; } = googlePayload.ExpirationTimeSeconds;

    /// <summary>
    /// Gets or sets not before claim that identifies the time (in seconds) before which the token MUST NOT be
    /// accepted for processing or <c>null</c>.
    /// </summary>
    public long? NotBeforeTimeSeconds { get; set; } = googlePayload.NotBeforeTimeSeconds;

    /// <summary>
    /// Gets or sets issued at claim that identifies the time (in seconds) at which the JWT was issued or 
    /// <c>null</c>.
    /// </summary>
    public long? IssuedAtTimeSeconds { get; set; } = googlePayload.IssuedAtTimeSeconds;

    /// <summary>
    /// Gets or sets JWT ID claim that provides a unique identifier for the JWT or <c>null</c>.
    /// </summary>
    public string JwtId { get; set; } = googlePayload.JwtId ?? string.Empty;

    /// <summary>
    /// The nonce value specified by the client during the authorization request.
    /// Must be present if a nonce was specified in the authorization request, otherwise this will not be present.
    /// </summary>
    public string Nonce { get; set; } = googlePayload.Nonce ?? string.Empty;

    /// <summary>
    /// Gets or sets type claim that is used to declare a type for the contents of this JWT Claims Set or 
    /// <c>null</c>.
    /// </summary>
    public string Type { get; set; } = googlePayload.Type ?? string.Empty;
}//Cls
