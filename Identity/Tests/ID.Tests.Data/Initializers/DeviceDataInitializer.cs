using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using TestingHelpers;

namespace ID.Tests.Data.Initializers;
public class DeviceDataInitializer
{
    //------------------------------------//

    public static List<TeamDevice> GenerateData(IEnumerable<TeamSubscription> subs)
    {
        var devices = new List<TeamDevice>();

        foreach (var sub in subs)
        {
            devices.Add(DeviceDataFactory.Create(
                Guid.NewGuid(),
                sub.Id,
                $"{sub.Name}_{RandomStringGenerator.Word()}_Device",
                $"{RandomStringGenerator.Sentence()}",
                RandomStringGenerator.Generate(20))
            );
        }

        return devices;
    }

    //------------------------------------//

}//Cls
