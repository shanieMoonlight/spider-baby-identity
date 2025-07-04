using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.ValueObjects;
using TestingHelpers;

namespace ID.Tests.Data.Factories;

public static class TeamDataFactory
{


    public static readonly Team AnyTeam = Team.CreateCustomerTeam(
            Name.Create("AnyTeam"),
            DescriptionNullable.Create("A bunch of randomers"),
            TeamCapacity.Create(10),
            MinTeamPosition.Create(1),
            MaxTeamPosition.Create(5)
        );


    //- - - - - - - - - - - - - - - - - - //

    public static List<Team> CreateMany(int count = 20)
    {
        return [.. IdGenerator.GetGuidIdsList(count).Select(id => Create(id))];
    }

    //- - - - - - - - - - - - - - - - - - //

    public static Team Create(
        Guid? id = null,
        string? name = null,
        string? description = null,
        HashSet<TeamSubscription>? subscriptions = null,
        TeamType? teamType = null,
        Guid? leaderId = null,
        AppUser? leader = null,
        HashSet<AppUser>? members = null,
        int? minPosition = null,
        int? maxPosition = null,
        int? capacity = null,
        string? administratorUsername = null,
        string? administratorId = null)
    {

        name ??= $"{RandomStringGenerator.Word()}";
        //description ??= $"{RandomStringGenerator.Sentence()})";
        id ??= Guid.NewGuid();
        administratorUsername ??= $"{RandomStringGenerator.FirstName()}{id}";
        administratorId ??= $"{RandomStringGenerator.Generate(20)}{id}";
        subscriptions ??= [];
        teamType ??= TeamType.maintenance;
        minPosition ??= 0;
        maxPosition ??= 5;


        var paramaters = new List<PropertyAssignment>()
           {
                new(nameof(Team.Name),  () => name ),
                new(nameof(Team.Description),  () => description ),
                new(nameof(Team.TeamType),  () => teamType ),
                //new PropertyAssignment("_teamSubscriptions",  () => subscriptions ),
                new(nameof(Team.Id),  () => id ),
                new(nameof(Team.LeaderId),  () => leader?.Id ?? leaderId ),
                new(nameof(Team.Leader),  () => leader ),
                new(nameof(Team.AdministratorUsername),  () => administratorUsername ),
                new(nameof(Team.AdministratorId),  () => administratorId ),
                new(nameof(Team.MinPosition),  () => minPosition ),
                new(nameof(Team.MaxPosition),  () => maxPosition ),
                new(nameof(Team.Capacity),  () => capacity )

            };

        members ??= [];
        //var membersList = members.ToList();
        if (leader is not null)
            members.Add(leader);
        //if (membersList.Count != 0)
        //    paramaters.Add(new PropertyAssignment(nameof(Team.Members), () => membersList));


        var team = ConstructorInvoker.CreateNoParamsInstance<Team>([.. paramaters]);

        NonPublicClassMembers.SetField(team, "_subscriptions", subscriptions);
        if (members.Count != 0)
            NonPublicClassMembers.SetField(team, "_members", members);

        return team;
    }

    //------------------------------------//

}//Cls

