using System.Security.Claims;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class ClaimsIdentityDataFactory
{

    //------------------------------------//

    public static List<ClaimsIdentity> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create())];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static ClaimsIdentity Create(
        IEnumerable<Claim>? claims = null,
        bool isAuthenticated = true)
    {

        claims ??= [];


        var paramaters = new List<PropertyAssignment>
        {
        };
        if (isAuthenticated)
            paramaters.Add(new PropertyAssignment("_authenticationType", () => "AuthenticationType"));

        var identity = ConstructorInvoker.CreateNoParamsInstance<ClaimsIdentity>([.. paramaters]);  

        identity.AddClaims(claims);

        return identity;
    }

    //------------------------------------//

}//Cls

