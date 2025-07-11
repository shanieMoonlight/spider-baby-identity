using ID.Infrastructure.Auth.JWT.Setup;
using ID.Jwt.KeyBuilder;

namespace ID.Infrastructure.Auth.JWT.Utils;
internal static class AsymmetricKeyPairExtensions
{

    internal static AsymmetricPemKeyPair ToPemPair(this AsymmetricXmlKeyPair xmlKeyPair)
    {
        return AsymmetricPemKeyPair.Create(
            XmlToPem.ConvertPublicKey(xmlKeyPair.PublicKeyXml),
            XmlToPem.ConvertPrivateKey(xmlKeyPair.PrivateKeyXml)
        );


    }

}
