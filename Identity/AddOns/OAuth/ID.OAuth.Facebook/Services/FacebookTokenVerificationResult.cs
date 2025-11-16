namespace ID.OAuth.Facebook.Services;

public sealed record FacebookTokenVerificationResult(bool IsValid, string? UserId, DateTimeOffset? ExpiresAt, string[] Scopes, string? Error)
{
    public static FacebookTokenVerificationResult Valid(string userId, DateTimeOffset? expiresAt, string[] scopes) =>
        new(true, userId, expiresAt, scopes, null);

    public static FacebookTokenVerificationResult Invalid(string error) =>
        new(false, null, null, [], error);
}

