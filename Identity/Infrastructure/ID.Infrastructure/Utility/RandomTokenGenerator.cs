using System.Security.Cryptography;

namespace ID.Infrastructure.Utility;
internal class RandomTokenGenerator
{
    public static string Generate(int minLength = 100, int maxLength = 120)
    {
        var tokenLength = RandomNumberGenerator.GetInt32(minLength, maxLength);
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(tokenLength));
    }

}
