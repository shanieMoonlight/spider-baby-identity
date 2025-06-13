using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;
using System.Reflection;

namespace ID.Domain.Tests.Entities.Teams.Validators;

public class LeaderUpdateValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Validate_WithValidTeamAndNewLeader_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.NewLeader.ShouldBe(newLeader);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenNewLeaderBelongsToOtherTeam_ShouldReturnFailure()
    {
        // Arrange
        var otherTeam = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create();
        newLeader.SetTeam(otherTeam); // Member belongs to different team

        var team = TeamDataFactory.Create();

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenNewLeaderIsAlreadyTheLeader_ShouldReturnFailure()
    {
        // Arrange
        var currentLeader = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(leader: currentLeader, members: [currentLeader]);

        // Act - Try to set the same person as leader again
        var result = TeamValidators.LeaderUpdate.Validate(team, currentLeader);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeEmpty();
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenNewLeaderBelongsToSameTeam_ShouldReturnSuccess()
    {
        // Arrange
        var existingMember = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(members: [existingMember]);
        existingMember.SetTeam(team); // Member belongs to same team

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, existingMember);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.NewLeader.ShouldBe(existingMember);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenNewLeaderHasNoTeam_ShouldReturnSuccess()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create(); // No team set (TeamId is empty)

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenTeamHasNoCurrentLeader_ShouldReturnSuccess()
    {
        // Arrange
        var team = TeamDataFactory.Create(); // No leader set
        var newLeader = AppUserDataFactory.Create();

        team.LeaderId.ShouldBeNull(); // Ensure no current leader

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenChangingFromOneLeaderToAnother_ShouldReturnSuccess()
    {
        // Arrange
        var currentLeader = AppUserDataFactory.Create();
        var newLeader = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(leader: currentLeader, members: [currentLeader, newLeader]);
        
        currentLeader.SetTeam(team);
        newLeader.SetTeam(team);

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.NewLeader.ShouldBe(newLeader);
    }

    //------------------------------------//

    [Theory]
    [InlineData(true)]  // NewLeader belongs to same team
    [InlineData(false)] // NewLeader has no team
    public void Validate_WithDifferentTeamMembershipScenarios_ShouldReturnSuccess(bool memberBelongsToTeam)
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create();
        
        if (memberBelongsToTeam)
        {
            newLeader.SetTeam(team);
        }
        // else: newLeader has no team (TeamId remains Guid.Empty)

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldHaveInternalConstructor()
    {
        // Arrange & Act
        var tokenType = typeof(TeamValidators.LeaderUpdate.Token);
        var constructors = tokenType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        // Assert
        constructors.ShouldHaveSingleItem();
        var constructor = constructors.First();
        constructor.IsAssembly.ShouldBeTrue(); // internal
        constructor.IsPublic.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldImplementIValidationToken()
    {
        // Arrange & Act
        var tokenType = typeof(TeamValidators.LeaderUpdate.Token);

        // Assert
        tokenType.GetInterfaces().ShouldContain(typeof(IValidationToken));
    }

    //------------------------------------//

    [Fact]
    public void Validate_SuccessfulValidation_TokenShouldContainCorrectData()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;

        token.Team.ShouldBe(team);
        token.NewLeader.ShouldBe(newLeader);

        // Verify token implements IValidationToken properly
        token.Team.ShouldBe(team);
        token.NewLeader.ShouldBe(newLeader);
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldHaveCorrectPropertyNames()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.LeaderUpdate.Validate(team, newLeader);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;

        // Verify property naming is consistent with LeaderUpdate context
        token.ShouldBeAssignableTo<IValidationToken>();
        //token.Should().HaveProperty("Team");
        token.NewLeader.ShouldBe(newLeader);
    }

    //------------------------------------//

    [Fact]
    public void Validate_MultipleValidationScenarios_ShouldReturnAppropriateResults()
    {
        // Arrange - Different team and member configurations
        var team1 = TeamDataFactory.Create();
        var team2 = TeamDataFactory.Create();
        
        var memberOfTeam1 = AppUserDataFactory.Create();
        memberOfTeam1.SetTeam(team1);
        
        var memberOfTeam2 = AppUserDataFactory.Create();
        memberOfTeam2.SetTeam(team2);
        
        var memberWithNoTeam = AppUserDataFactory.Create();

        // Act & Assert - Member from different team should fail
        var result1 = TeamValidators.LeaderUpdate.Validate(team1, memberOfTeam2);
        result1.Succeeded.ShouldBeFalse();

        // Act & Assert - Member from same team should succeed
        var result2 = TeamValidators.LeaderUpdate.Validate(team1, memberOfTeam1);
        result2.Succeeded.ShouldBeTrue();

        // Act & Assert - Member with no team should succeed
        var result3 = TeamValidators.LeaderUpdate.Validate(team1, memberWithNoTeam);
        result3.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ConstructorParameters_ShouldMatchExpectedSignature()
    {
        // Arrange & Act
        var tokenType = typeof(TeamValidators.LeaderUpdate.Token);
        var constructor = tokenType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();
        var parameters = constructor.GetParameters();

        // Assert
        parameters.Length.ShouldBe(2);
        parameters[0].ParameterType.ShouldBe(typeof(Team));
        parameters[1].ParameterType.ShouldBe(typeof(AppUser));
        parameters[0].Name.ShouldBe("team");
        parameters[1].Name.ShouldBe("member");
    }

    //------------------------------------//

}//Cls