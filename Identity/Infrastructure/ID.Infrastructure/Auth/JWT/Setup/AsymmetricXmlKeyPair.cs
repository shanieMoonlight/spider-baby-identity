using ID.Infrastructure.Auth.JWT.Utils;

namespace ID.Infrastructure.Auth.JWT.Setup;
public class AsymmetricXmlKeyPair
{
    public string PublicKeyXml { get; private set; } = string.Empty;
    public string PrivateKeyXml { get; private set; } = string.Empty;

    //--------------------------//

    private AsymmetricXmlKeyPair(string publicKeyXml, string privateKeyXml)
    {
        PublicKeyXml = publicKeyXml;
        PrivateKeyXml = privateKeyXml;
    }


    //--------------------------//

    public static AsymmetricXmlKeyPair Create(string publicKeyXml, string privateKeyXml)
    {
        if (string.IsNullOrWhiteSpace(publicKeyXml))
            throw new ArgumentException("Public key XML cannot be null or empty.", nameof(publicKeyXml));

        if (string.IsNullOrWhiteSpace(privateKeyXml))
            throw new ArgumentException("Private key XML cannot be null or empty.", nameof(privateKeyXml));


        if (!publicKeyXml.IsValidRsaPublicKeyXml())
            throw new ArgumentException($"Invalid Public Key {publicKeyXml}", nameof(publicKeyXml));

        if (!privateKeyXml.IsValidRsaPrivateKeyXml())
            throw new ArgumentException($"Invalid Private Key {privateKeyXml}", nameof(privateKeyXml));


        return new AsymmetricXmlKeyPair(publicKeyXml, privateKeyXml);
    }

}//Cls
