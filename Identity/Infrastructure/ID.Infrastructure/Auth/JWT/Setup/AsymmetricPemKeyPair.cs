using ID.Infrastructure.Auth.JWT.Utils;

namespace ID.Infrastructure.Auth.JWT.Setup;
public class AsymmetricPemKeyPair
{
    public string PublicKey { get; private set; } = string.Empty;
    
    public string PrivateKey { get; private set; } = string.Empty;



    //--------------------------//

    private AsymmetricPemKeyPair(string publicKeyPem, string privateKeyPem)
    {
        PublicKey = publicKeyPem;
        PrivateKey = privateKeyPem;
    }


    //--------------------------//

    public static AsymmetricPemKeyPair Create(string publicKeyPem, string privateKeyPem)
    {
        if (string.IsNullOrWhiteSpace(publicKeyPem))
            throw new ArgumentException("Public key PEM cannot be null or empty.", nameof(publicKeyPem));

        if (string.IsNullOrWhiteSpace(privateKeyPem))
            throw new ArgumentException("Private key PEM cannot be null or empty.", nameof(privateKeyPem));

        if (!publicKeyPem.IsPublicKey())
            throw new ArgumentException($"Invalid Public Key {publicKeyPem}", nameof(publicKeyPem));

        if (!privateKeyPem.IsPrivateKey())
            throw new ArgumentException($"Invalid Private Key {privateKeyPem}", nameof(privateKeyPem));

        return new AsymmetricPemKeyPair(publicKeyPem, privateKeyPem);
    }

}//Cls
