using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Factories.Dtos;
using TestingHelpers;
using static ID.Domain.Utility.Messages.IDMsgs.Error;

namespace ID.Tests.Data.Initializers;
public class UserInitializer
{
    //------------------------------------//

    public static List<AppUser> GenerateData(IEnumerable<Team> teams)
    {

        List<AppUser> users = [];


        foreach (var team in teams)
        {
            Guid userId = Guid.NewGuid();
            if (team.TeamType == TeamType.super)
            {
                users.Add(
                    AppUserDataFactory.Create(
                        team.Id,
                        userId,
                        "Clarke",
                        "Kent",
                        "superLeader",
                        "clarke@krypton.com",
                        "066 666 666 66",
                        "P@$5w0rd!!!",
                        identityAddress: IdentityAddressDataFactory.Create(userId))
                    );
            }
            else if (team.TeamType == TeamType.maintenance)
            {
                users.Add(
                    AppUserDataFactory.Create(
                        team.Id,
                        userId,
                        "Bob",
                        "TheBuilder",
                        "mntcLeader",
                        "mntcman@krypton.com",
                        "066 123 666 66",
                        "P@$5w0rd!!!",
                        identityAddress: IdentityAddressDataFactory.Create(userId))
                    );
            }
            else
            {
                users.Add(
                    AppUserDataFactory.Create(
                        team.Id,
                        userId,
                       MyRandomDataGenerator.FirstName(),
                       MyRandomDataGenerator.LastName(),
                       MyRandomDataGenerator.Username(),
                       MyRandomDataGenerator.Email(),
                       MyRandomDataGenerator.Phone(),
                        "P@$5w0rd!!!",
                        identityAddress: IdentityAddressDataFactory.Create(userId))
                    );
            }

        }


        return users;
    }

    //------------------------------------//

}//Cls
