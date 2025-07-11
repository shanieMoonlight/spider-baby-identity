namespace ID.Application.JWT;
public interface IJwtCurrentKeyService
{
    /// <summary>
    /// Get the current public key for JWT signing
    /// </summary>
    Task<string?> GetPublicSigningKeyAsync();
}
