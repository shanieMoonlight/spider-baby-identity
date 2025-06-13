using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ID.Domain.Claims.Utils;

/// <summary>
/// Used to find a Claim in a list of Claims
/// </summary>
public class ClaimEqualityComparer : IEqualityComparer<Claim>
{
    private static ClaimEqualityComparer? _instance;

    //---------------------------------------------//

    public bool Equals([AllowNull] Claim claim1, [AllowNull] Claim claim2)
    {
        if (claim1 == null || claim2 == null)
            return false;

        if (claim1.Type != claim2.Type)
            return false;

        return claim1.Value == claim2.Value;

    }

    //---------------------------------------------//

    public int GetHashCode([DisallowNull] Claim claim) => claim.GetHashCode();

    //---------------------------------------------//

    public static ClaimEqualityComparer GetInstance()
    {
        _instance ??= new ClaimEqualityComparer();
        return _instance;
    }

    //---------------------------------------------//


}//Cls