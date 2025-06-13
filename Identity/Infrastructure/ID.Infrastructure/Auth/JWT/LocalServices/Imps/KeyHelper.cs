using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Imps;

public class KeyHelper(IOptions<JwtOptions> jwtOptions) : IKeyHelper
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public RSAParameters ParseXmlString(string xml)
    {
        RSAParameters parameters = new();

        System.Xml.XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xml);

        if (xmlDoc.DocumentElement is null || !xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            throw new ArgumentException("Invalid XML RSA key.");

        foreach (System.Xml.XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        {
            byte[]? value = string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText);
            switch (node.Name)
            {
                case "Modulus":
                    parameters.Modulus = value;
                    break;
                case "Exponent":
                    parameters.Exponent = value;
                    break;
                case "P":
                    parameters.P = value;
                    break;
                case "Q":
                    parameters.Q = value;
                    break;
                case "DP":
                    parameters.DP = value;
                    break;
                case "DQ":
                    parameters.DQ = value;
                    break;
                case "InverseQ":
                    parameters.InverseQ = value;
                    break;
                case "D":
                    parameters.D = value;
                    break;
            }
        }

        return parameters;
    }

    public RsaSecurityKey BuildRsaSigningKey(string xml)
    {
        RSAParameters parameters = ParseXmlString(xml);
        RSACryptoServiceProvider rsaProvider = new(2048);
        rsaProvider.ImportParameters(parameters);

        return new RsaSecurityKey(rsaProvider);
    }

    public RsaSecurityKey BuildPrivateRsaSigningKey()
        => BuildRsaSigningKey(_jwtOptions.AsymmetricTokenPrivateKey_Xml);

    public RsaSecurityKey BuildPublicRsaSigningKey()
        => BuildRsaSigningKey(_jwtOptions.AsymmetricTokenPublicKey_Xml);

    public SymmetricSecurityKey BuildSymmetricSigningKey() =>
        new(Encoding.ASCII.GetBytes(_jwtOptions.SymmetricTokenSigningKey));

    public SecurityKey BuildSigningKey() =>
        _jwtOptions.UseAsymmetricCrypto
        ? BuildPrivateRsaSigningKey()
        : BuildSymmetricSigningKey();

    public SecurityKey BuildValidationSigningKey() =>
        _jwtOptions.UseAsymmetricCrypto
        ? BuildPublicRsaSigningKey()
        : BuildSymmetricSigningKey();

    public string ExportPublicKey()
    {
        RSAParameters parameters = ParseXmlString(_jwtOptions.AsymmetricTokenPublicKey_Xml);

        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        writer.Write((byte)0x30); // SEQUENCE
        using (MemoryStream innerStream = new())
        {
            using BinaryWriter innerWriter = new(innerStream);
            innerWriter.Write((byte)0x30); // SEQUENCE
            EncodeLength(innerWriter, 13);
            innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
            byte[] rsaEncryptionOid = [0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01];
            EncodeLength(innerWriter, rsaEncryptionOid.Length);
            innerWriter.Write(rsaEncryptionOid);
            innerWriter.Write((byte)0x05); // NULL
            EncodeLength(innerWriter, 0);
            innerWriter.Write((byte)0x03); // BIT STRING
            using (MemoryStream bitStringStream = new())
            {
                BinaryWriter bitStringWriter = new(bitStringStream);
                bitStringWriter.Write((byte)0x00); // # of unused bits
                bitStringWriter.Write((byte)0x30); // SEQUENCE
                using (MemoryStream paramsStream = new())
                {
                    BinaryWriter paramsWriter = new(paramsStream);
                    EncodeIntegerBigEndian(paramsWriter, parameters.Modulus!); // Modulus
                    EncodeIntegerBigEndian(paramsWriter, parameters.Exponent!); // Exponent
                    int paramsLength = (int)paramsStream.Length;
                    EncodeLength(bitStringWriter, paramsLength);
                    bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                }
                int bitStringLength = (int)bitStringStream.Length;
                EncodeLength(innerWriter, bitStringLength);
                innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
            }
            int length = (int)innerStream.Length;
            EncodeLength(writer, length);
            writer.Write(innerStream.GetBuffer(), 0, length);
        }

        char[] base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
        StringBuilder sb = new();
        sb.AppendLine("-----BEGIN PUBLIC KEY-----");
        for (int i = 0; i < base64.Length; i += 64)
        {
            int maxJ = Math.Min(64 + i, base64.Length);
            for (int j = i; j < maxJ; j++)
                sb.Append(base64[j]);
            sb.AppendLine();
        }
        sb.AppendLine("-----END PUBLIC KEY-----");

        return sb.ToString();
    }

    private static void EncodeLength(BinaryWriter stream, int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative");

        if (length < 0x80)
        {
            // Short form
            stream.Write((byte)length);
        }
        else
        {
            // Long form
            int temp = length;
            int bytesRequired = 0;
            while (temp > 0)
            {
                temp >>= 8;
                bytesRequired++;
            }

            stream.Write((byte)(bytesRequired | 0x80));
            for (int i = bytesRequired - 1; i >= 0; i--)
            {
                stream.Write((byte)(length >> 8 * i & 0xff));
            }
        }
    }

    private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
    {
        stream.Write((byte)0x02); // INTEGER
        int prefixZeros = 0;

        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] != 0) break;
            prefixZeros++;
        }

        if (value.Length - prefixZeros == 0)
        {
            EncodeLength(stream, 1);
            stream.Write((byte)0);
            return;
        }

        if (forceUnsigned && value[prefixZeros] > 0x7f)
        {
            // Add a prefix zero to force unsigned if the MSB is 1
            EncodeLength(stream, value.Length - prefixZeros + 1);
            stream.Write((byte)0);
        }
        else
        {
            EncodeLength(stream, value.Length - prefixZeros);
        }
        for (int i = prefixZeros; i < value.Length; i++)
        {
            stream.Write(value[i]);
        }
    }
}
