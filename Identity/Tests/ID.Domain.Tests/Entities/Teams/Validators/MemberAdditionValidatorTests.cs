using ID.Domain.Entities.Teams.Validators;
using ID.Tests.Data.Factories;
using Shouldly;
using System.Reflection;

namespace ID.Domain.Tests.Entities.Teams.Validators;

public class MemberAdditionValidatorTests
{


    [Fact]
    public void Validate_WithValidTeamAndMember_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var team = TeamDataFactory.Create(capacity: 5);
        var member = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, member);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Team.ShouldBe(team);
        result.Value.Member.ShouldBe(member);
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenTeamAtCapacity_ShouldReturnFailure()
    {
        // Arrange
        var member1 = AppUserDataFactory.Create();
        var member2 = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(capacity: 2, members: [member1, member2]);

        var newMember = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, newMember);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("capacity");
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenTeamHasNoCapacityLimit_ShouldReturnSuccess()
    {
        // Arrange
        var team = TeamDataFactory.Create(capacity: null); // No capacity limit
        var member = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, member);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenMemberAlreadyOnTeam_ShouldReturnFailure()
    {
        // Arrange
        var existingMember = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(members: [existingMember]);

        // Act - Try to add the same member again
        var result = TeamValidators.MemberAddition.Validate(team, existingMember);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("already");
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Fact]
    public void Validate_WhenMemberBelongsToOtherTeam_ShouldReturnFailure()
    {
        // Arrange
        var otherTeam = TeamDataFactory.Create();
        var member = AppUserDataFactory.Create();
        member.SetTeam(otherTeam); // Member belongs to different team

        var team = TeamDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, member);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

    [Theory]
    [InlineData(1, 0)] // Capacity 1, 0 members - should succeed
    [InlineData(3, 2)] // Capacity 3, 2 members - should succeed
    [InlineData(5, 4)] // Capacity 5, 4 members - should succeed
    public void Validate_WhenTeamUnderCapacity_ShouldReturnSuccess(int capacity, int existingMemberCount)
    {
        // Arrange
        var existingMembers = Enumerable.Range(0, existingMemberCount)
            .Select(_ => AppUserDataFactory.Create())
            .ToList();

        var team = TeamDataFactory.Create(capacity: capacity, members: [..existingMembers]);
        var newMember = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, newMember);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

    [Theory]
    [InlineData(1, 1)] // Capacity 1, 1 member - should fail
    [InlineData(3, 3)] // Capacity 3, 3 members - should fail
    [InlineData(5, 5)] // Capacity 5, 5 members - should fail
    public void Validate_WhenTeamAtOrOverCapacity_ShouldReturnFailure(int capacity, int existingMemberCount)
    {
        // Arrange
        var existingMembers = Enumerable.Range(0, existingMemberCount)
            .Select(_ => AppUserDataFactory.Create())
            .ToList();

        var team = TeamDataFactory.Create(capacity: capacity, members: [.. existingMembers]);
        var newMember = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, newMember);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("capacity");
    }

    //------------------------------------//

    [Fact]
    public void Validate_TokenShouldHaveInternalConstructor()
    {
        // Arrange & Act
        var tokenType = typeof(TeamValidators.MemberAddition.Token);
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
        var tokenType = typeof(TeamValidators.MemberAddition.Token);

        // Assert
        tokenType.GetInterfaces().ShouldContain(typeof(IValidationToken));
    }

    //------------------------------------//

    [Fact]
    public void Validate_SuccessfulValidation_TokenShouldContainCorrectData()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var member = AppUserDataFactory.Create();

        // Act
        var result = TeamValidators.MemberAddition.Validate(team, member);

        // Assert
        result.Succeeded.ShouldBeTrue();
        var token = result.Value!;

        token.Team.ShouldBe(team);
        token.Member.ShouldBe(member);

        // Verify token implements IValidationToken properly
        token.Team.ShouldBe(team);
        token.Member.ShouldBe(member);
    }

    //------------------------------------//

    [Fact]
    public void Validate_MultipleValidationErrors_ShouldReturnFirstError()
    {
        // Arrange - Create scenario with multiple potential errors
        var existingMember = AppUserDataFactory.Create();
        var team = TeamDataFactory.Create(capacity: 1, members: [existingMember]); // At capacity

        // Try to add the same existing member (two validation failures)
        var result = TeamValidators.MemberAddition.Validate(team, existingMember);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNullOrEmpty();
        // Should get the first error encountered (likely capacity or already member)
    }


}//Cls