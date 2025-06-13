using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Members;
using ID.Tests.Data.Factories;
using Shouldly;
using Xunit;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Members;

public class MemberExistsEmailSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamId: Guid.NewGuid(), email: "test@example.com");

        // Act
        var spec = new MemberExistsEmailSpec<AppUser>(user.Email);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(user).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongEmail()
    {
        // Arrange
        var user = AppUserDataFactory.Create(teamId: Guid.NewGuid(), email: "test@example.com");
        var differentEmail = "different@example.com";

        // Act
        var spec = new MemberExistsEmailSpec<AppUser>(differentEmail);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(user).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        string? email = null;

        // Act
        var spec = new MemberExistsEmailSpec<AppUser>(email);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
