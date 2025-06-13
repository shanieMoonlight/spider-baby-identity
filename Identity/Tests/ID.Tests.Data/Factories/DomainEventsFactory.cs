using ClArch.ValueObjects;
using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.ValueObjects;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

//=================================================================//

public sealed record Test_Team_DomainEvent(Team Team) : IIdDomainEvent
{
    public string? UserInfo { get; set; } = "This is a test homie";
}

public sealed record Test_Device_DomainEvent(TeamDevice Dvc) : IIdDomainEvent
{
    public string? UserInfo { get; set; } = "This is another test homie";
}


public sealed record Test_Id_DomainEvent(Guid SomeId) : IIdDomainEvent
{
    public string? UserInfo { get; set; } = "This is another test homie";
}
//=================================================================//

internal class DomainEventsFactory
{
    public static IIdDomainEvent CreateRandom() =>
        new Random().Next(3) switch
        {
            0 => GetTeamEvent(),
            1 => GetIdEvent(),
            2 => GetDeviceEvent(),
            _ => GetIdEvent(),
        };

    //------------------------------------//

    private static Test_Team_DomainEvent GetTeamEvent()
    {
        var team = Team.CreateCustomerTeam(
            Name.Create($"{RandomStringGenerator.Generate(20)}"),
            DescriptionNullable.Create($"{RandomStringGenerator.Generate(20)}"),
            TeamCapacity.Create(10),
            MinTeamPosition.Create(1),
            MaxTeamPosition.Create(5));

        return new(team);
    }

    //- - - - - - - - - - - - - - - - - - //

    private static Test_Id_DomainEvent GetIdEvent() =>
        new(Guid.NewGuid());

    //- - - - - - - - - - - - - - - - - - //

    private static Test_Device_DomainEvent GetDeviceEvent()
    {
        var dvc = TeamDevice.Create(
            SubscriptionDataFactory.Create(),
            Name.Create($"{RandomStringGenerator.Generate(20)}"),
            DescriptionNullable.Create($"{RandomStringGenerator.Generate(20)}"),
            UniqueId.Create(RandomStringGenerator.Generate(20))
            );

        return new(dvc);
    }

    //------------------------------------//

}//Cls

//=================================================================//
