using Microsoft.IdentityModel.Tokens;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Abs;

public interface IKeyProvider
{

    /// <summary>
    /// Builds an asymmetric signing key using the private key from JWT options.
    /// </summary>
    /// <returns>RSA security key for signing.</returns>
    RsaSecurityKey GetPrivateRsaSigningKey();

    /// <summary>
    /// Builds a symmetric signing key from JWT options.
    /// </summary>
    /// <returns>Symmetric security key.</returns>
    SymmetricSecurityKey GetSymmetricSigningKey();


    /// <summary>
    /// Builds a validation signing key based on JWT options preferences (asymmetric or symmetric).
    /// </summary>
    /// <returns>Security key for validating tokens.</returns>
    SecurityKey GetValidationSigningKey();


    /// <summary>
    /// Builds a List of validation signing keys based on JWT options preferences (asymmetric or symmetric). Includes legacy keys.
    /// </summary>
    /// <returns>Security key for validating tokens.</returns>
    List<SecurityKey> GetValidationSigningKeys();

    /// <summary>
    /// Exports the public key in PEM format.
    /// </summary>
    /// <returns>Public key in PEM format.</returns>
    string ExportPublicKey();

    /// <summary>
    /// Gets a list of asymmetric validation signing keys, including legacy keys.
    /// </summary>
    List<RsaSecurityKey> GetAsymmetricValidationSigningKeys();
}
