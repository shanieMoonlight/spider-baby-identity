using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Abs;

public interface IKeyHelper
{
    /// <summary>
    /// Parses an XML string to RSA parameters.
    /// </summary>
    /// <param name="xml">The XML string containing RSA key information.</param>
    /// <returns>RSA parameters parsed from the XML.</returns>
    RSAParameters ParseXmlString(string xml);

    /// <summary>
    /// Builds an asymmetric signing key from XML.
    /// </summary>
    /// <param name="xml">Private key in XML format.</param>
    /// <returns>RSA security key for signing.</returns>
    RsaSecurityKey BuildRsaSigningKey(string xml);

    /// <summary>
    /// Builds an asymmetric signing key using the private key from JWT options.
    /// </summary>
    /// <returns>RSA security key for signing.</returns>
    RsaSecurityKey BuildPrivateRsaSigningKey();

    /// <summary>
    /// Builds an asymmetric signing key using the public key from JWT options.
    /// </summary>
    /// <returns>RSA security key for validation.</returns>
    RsaSecurityKey BuildPublicRsaSigningKey();

    /// <summary>
    /// Builds a symmetric signing key from JWT options.
    /// </summary>
    /// <returns>Symmetric security key.</returns>
    SymmetricSecurityKey BuildSymmetricSigningKey();

    /// <summary>
    /// Builds a signing key based on JWT options preferences (asymmetric or symmetric).
    /// </summary>
    /// <returns>Security key for signing tokens.</returns>
    SecurityKey BuildSigningKey();

    /// <summary>
    /// Builds a validation signing key based on JWT options preferences (asymmetric or symmetric).
    /// </summary>
    /// <returns>Security key for validating tokens.</returns>
    SecurityKey BuildValidationSigningKey();

    /// <summary>
    /// Exports the public key in PEM format.
    /// </summary>
    /// <returns>Public key in PEM format.</returns>
    string ExportPublicKey();
}
