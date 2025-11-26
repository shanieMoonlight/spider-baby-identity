using System.Security.Cryptography;

namespace ID.Infrastructure.Utility;
internal class RandomTokenGenerator
{
    public static string Generate(int minLength = 100, int maxLength = 120)
    {
        var tokenLength = RandomNumberGenerator.GetInt32(minLength, maxLength);
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(tokenLength));
    }


    public static string GenerateHashingSelector()
    {
        var bytes = RandomNumberGenerator.GetBytes(12);
        var s = Convert.ToBase64String(bytes);
        s = s.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return s;
    }

}//Cls
