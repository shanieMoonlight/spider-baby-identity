using System.Security.Claims;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class ClaimsPrincipalDataFactory
{

    //------------------------------------//

    public static List<ClaimsPrincipal> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create())];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static ClaimsPrincipal Create(IEnumerable<Claim>? claims = null)
    {

        claims ??= [];

        var identity = new ClaimsIdentity(claims);

        var paramaters = new[]
            {
                new PropertyAssignment("_identities",  () => new List<ClaimsIdentity>(){ identity } )
            };


        return ConstructorInvoker.CreateNoParamsInstance<ClaimsPrincipal>(paramaters);
    }

    //------------------------------------//

}//Cls

