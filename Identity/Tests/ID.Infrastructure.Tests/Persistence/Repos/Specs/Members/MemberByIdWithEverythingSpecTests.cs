using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Members.WithEverything;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Members;

public class MemberByEmailWithEverythingSpecTests
{
    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        AppUser user =  AppUserDataFactory.Create(Guid.NewGuid());

        // Act
        var spec = new MemberByEmailWithEverythingSpec<AppUser>(user.Email); //Same Id
        var what = spec.TESTING_GetCriteria().Compile()(user);
        // Assert
        spec.TESTING_GetCriteria().ShouldNotBeNull();
        spec.TESTING_GetCriteria().Compile()(user).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongId()
    {
        // Arrange
        var differentId = Guid.NewGuid();
        AppUser user = AppUserDataFactory.Create(Guid.NewGuid());

        // Act
        var spec = new MemberByEmailWithEverythingSpec<AppUser>("other@email.com");
        var what = spec.TESTING_GetCriteria().Compile()(user);
        // Assert
        spec.TESTING_GetCriteria().ShouldNotBeNull();
        spec.TESTING_GetCriteria().Compile()(user).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        string? email = null;

        // Act
        var spec = new MemberByEmailWithEverythingSpec<AppUser>(email);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//

}