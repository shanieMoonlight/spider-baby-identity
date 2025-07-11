using ID.Infrastructure.Auth.JWT.Utils;
using Microsoft.IdentityModel.Tokens;

namespace ID.Infrastructure.Tests.Auth.JWT.Utils;

public class KidIssuerSigningKeyResolverTests
{

    [Fact]
    public void ReturnsAllKeys_WhenKidIsNullOrEmpty()
    {
        var keys = new List<SecurityKey> { CreateKey("a"), CreateKey("b") };
        var resolver = new KidIssuerSigningKeyResolver(keys);
        var result = resolver.ResolveSigningKey("token", null!, null!, null!);
        Assert.Equal(keys, result);
        result = resolver.ResolveSigningKey("token", null!, "", null!);
        Assert.Equal(keys, result);
    }


    //-----------------------------//


    [Fact]
    public void ReturnsMatchingKey_WhenKidMatches()
    {
        var keys = new List<SecurityKey> { CreateKey("a"), CreateKey("b") };
        var resolver = new KidIssuerSigningKeyResolver(keys);
        var result = resolver.ResolveSigningKey("token", null!, "b", null!);
        Assert.Single(result);
        Assert.Equal("b", result.First().KeyId);
    }


    //-----------------------------//


    [Fact]
    public void ReturnsEmpty_WhenNoKidMatches()
    {
        var keys = new List<SecurityKey> { CreateKey("a"), CreateKey("b") };
        var resolver = new KidIssuerSigningKeyResolver(keys);
        var result = resolver.ResolveSigningKey("token", null!, "c", null!);
        Assert.Empty(result);
    }


    //-----------------------------//


    [Fact]
    public void HandlesMultipleKeysWithSameKid()
    {
        var keys = new List<SecurityKey> { CreateKey("a"), CreateKey("a"), CreateKey("b") };
        var resolver = new KidIssuerSigningKeyResolver(keys);
        var result = resolver.ResolveSigningKey("token", null!, "a", null!);
        Assert.Equal(2, result.Count());
        Assert.All(result, k => Assert.Equal("a", k.KeyId));
    }


    //-----------------------------//


    private static SymmetricSecurityKey CreateKey(string kid)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes($"key-{kid}"))
        {
            KeyId = kid
        };
        return key;
    }

}//Cls
