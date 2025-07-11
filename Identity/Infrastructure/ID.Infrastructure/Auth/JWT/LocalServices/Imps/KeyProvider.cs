using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Auth.JWT.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Imps;

public class KeyProvider(
    IOptions<JwtOptions> jwtOptions,
    IKeyIdBuilder _kidBuilder)
    : IKeyProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly ConcurrentDictionary<string, string> _pemToKid = new();

    //----------------------------//


    public RsaSecurityKey GetPrivateRsaSigningKey()
    {
        var keyPair = _jwtOptions.CurrentAsymmetricKeyPair
            ?? throw new InvalidOperationException("Current asymmetric key pair is not set in JWT options.");
        return BuildAsymmetricSigningKeyFromPrivatePem(keyPair);
    }


    //- - - - - - - - - - - - - - //


    public SymmetricSecurityKey GetSymmetricSigningKey() =>
        new(Encoding.ASCII.GetBytes(_jwtOptions.SymmetricTokenSigningKey));


    //- - - - - - - - - - - - - - //


    public SecurityKey GetValidationSigningKey() =>
        _jwtOptions.UseAsymmetricCrypto
        ? BuildAsymmetricPublicSigningKey()
        : GetSymmetricSigningKey();


    //- - - - - - - - - - - - - - //


    public List<SecurityKey> GetValidationSigningKeys()
    {
        if (!_jwtOptions.UseAsymmetricCrypto)
            return [GetSymmetricSigningKey()];


        var currentKeyPair = _jwtOptions.CurrentAsymmetricKeyPair
            ?? throw new InvalidOperationException("Current asymmetric key pair is not set in JWT options.");

        List<AsymmetricPemKeyPair> assymetricPublicKeys = [
            currentKeyPair,
            .._jwtOptions.LegacyAsymmetricKeyPairs
            ];

        return [.. assymetricPublicKeys
                .Select(keyPair => (SecurityKey)BuildAsymmetricSigningKeyFromPublicPem(keyPair.PublicKey))
                ];
    }


    //- - - - - - - - - - - - - - //


    public List<RsaSecurityKey> GetAsymmetricValidationSigningKeys()
    {
        if (!_jwtOptions.UseAsymmetricCrypto)
            return [];

        var currentKeyPair = _jwtOptions.CurrentAsymmetricKeyPair;

        if (currentKeyPair is null)
            return [];

        List<AsymmetricPemKeyPair> assymetricPublicKeys = [currentKeyPair, .. _jwtOptions.LegacyAsymmetricKeyPairs];

        return [.. assymetricPublicKeys.Select(keyPair => BuildAsymmetricSigningKeyFromPublicPem(keyPair.PublicKey))];
    }


    //- - - - - - - - - - - - - - //


    public string ExportPublicKey() =>
        _jwtOptions.CurrentAsymmetricKeyPair?.PublicKey ?? "";


    //----------------------------//


    private string GetOrAddKidForPem(string publicPem) =>
        _pemToKid.GetOrAdd(publicPem, _kidBuilder.GenerateKidFromPem);


    //- - - - - - - - - - - - - - //


    private RsaSecurityKey BuildAsymmetricPublicSigningKey()
    {
        var keyPair = _jwtOptions.CurrentAsymmetricKeyPair
            ?? throw new InvalidOperationException("Current asymmetric key pair is not set in JWT options.");
        return BuildAsymmetricSigningKeyFromPublicPem(keyPair.PublicKey);
    }


    //- - - - - - - - - - - - - - //


    private RsaSecurityKey BuildAsymmetricSigningKeyFromPublicPem(string publicPem)
    {
        // Remove PEM header/footer and whitespace
        var publicKey = publicPem.RemovePublicPemHeaderFooter();

        var keyBytes = Convert.FromBase64String(publicKey);

        var rsa = RSA.Create();
        var isRsaKey = publicPem.Contains(PemKeyConstants.PUBLIC_PEM_RSA_BEGIN);
        if (isRsaKey)
            rsa.ImportRSAPublicKey(keyBytes, out _); // PKCS#1
        else
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _); //

        return new RsaSecurityKey(rsa)
        {
            KeyId = GetOrAddKidForPem(publicPem)
        };

    }


    //- - - - - - - - - - - - - - //


    private RsaSecurityKey BuildAsymmetricSigningKeyFromPrivatePem(AsymmetricPemKeyPair keyPair)
    {
        var privateKey = keyPair.PrivateKey.RemovePrivatePemHeaderFooter();

        var keyBytes = Convert.FromBase64String(privateKey);

        var rsa = RSA.Create();

        var isRsaKey = keyPair.PrivateKey.Contains(PemKeyConstants.PRIVATE_PEM_RSA_BEGIN);

        if (isRsaKey)
            rsa.ImportRSAPrivateKey(keyBytes, out _);
        else
            rsa.ImportPkcs8PrivateKey(keyBytes, out _);

        return new RsaSecurityKey(rsa)
        {
            KeyId = GetOrAddKidForPem(keyPair.PublicKey)
        };

    }


}//Cls
