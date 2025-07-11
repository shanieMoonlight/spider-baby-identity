namespace ID.Infrastructure.Auth.JWT.Utils;
internal static class PemExtensions
{
    private static readonly string[] _publicHeaders = [PemKeyConstants.PUBLIC_PEM_BEGIN, PemKeyConstants.PUBLIC_PEM_RSA_BEGIN];
    private static readonly string[] _publicFooters = [PemKeyConstants.PUBLIC_PEM_END, PemKeyConstants.PUBLIC_PEM_RSA_END];
    private static readonly string[] _privateHeaders = [PemKeyConstants.PRIVATE_PEM_BEGIN, PemKeyConstants.PRIVATE_PEM_RSA_BEGIN];
    private static readonly string[] _privateFooters = [PemKeyConstants.PRIVATE_PEM_END, PemKeyConstants.PRIVATE_PEM_RSA_END];


    //-----------------------------//


    public static string RemovePublicPemHeaderFooter(this string pem)
    {
        if (string.IsNullOrWhiteSpace(pem))
            throw new ArgumentException("PEM string cannot be null or empty.", nameof(pem));
        
        pem = pem.Trim();

        foreach (var h in _publicHeaders) 
                pem = pem.Replace(h, "");
        foreach (var f in _publicFooters) 
            pem = pem.Replace(f, "");
        
        return pem.Replace("\r", "")
            .Replace("\n", "")
            .Trim();
    }



    public static string RemovePrivatePemHeaderFooter(this string pem)
    {
        if (string.IsNullOrWhiteSpace(pem))
            throw new ArgumentException("PEM string cannot be null or empty.", nameof(pem));
        
        pem = pem.Trim();
        
        foreach (var h in _privateHeaders) 
            pem = pem.Replace(h, "");
        foreach (var f in _privateFooters) 
            pem = pem.Replace(f, "");

        return pem.Replace("\r", "")
            .Replace("\n", "")
            .Trim();
    }

    //-----------------------------//

    public static bool IsPemKey(this string pem) =>
        !string.IsNullOrWhiteSpace(pem) && (pem.IsPublicKey() || pem.IsPrivateKey());


    public static bool IsPrivateKey(this string pem) =>
        !string.IsNullOrWhiteSpace(pem) && _privateHeaders.Any(h => pem.Trim().StartsWith(h));


    public static bool IsPublicKey(this string pem) =>
        !string.IsNullOrWhiteSpace(pem) && _publicHeaders.Any(h => pem.Trim().StartsWith(h));


    public static bool IsRsaKey(this string pem) =>
        pem.IsRsaPublicKey() || pem.IsRsaPrivateKey();


    public static bool IsRsaPublicKey(this string pem) =>
        !string.IsNullOrWhiteSpace(pem) && pem.Trim().StartsWith(PemKeyConstants.PUBLIC_PEM_RSA_BEGIN);


    public static bool IsRsaPrivateKey(this string pem) =>
        !string.IsNullOrWhiteSpace(pem) && pem.Trim().StartsWith(PemKeyConstants.PRIVATE_PEM_RSA_BEGIN);

}
