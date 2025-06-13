using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Factories.Dtos;

namespace ID.Tests.Data.Initializers;
public class IdentityAddressDataInitializer
{
    //------------------------------------//

    public static List<IdentityAddress> GenerateData(IEnumerable<AppUser> users)
    {
        var addresses = new List<IdentityAddress>();

        foreach (var user in users)
        {
            addresses.Add(IdentityAddressDataFactory.Create(user.Id));
        }

        return addresses;
    }

    //------------------------------------//

}//Cls
