using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Members;
using ID.Tests.Data.Factories;
using Shouldly;
using static ID.Domain.Utility.Messages.IDMsgs.Error;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Members;

public class MemberExistsIdSpecTests
{
    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        AppUser user =  AppUserDataFactory.Create(Guid.NewGuid());

        // Act
        var spec = new MemberExistsIdSpec<AppUser>(user.Id); //Same Id
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
        var spec = new MemberExistsIdSpec<AppUser>(differentId);
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
        Guid? id = null;

        // Act
        var spec = new MemberExistsIdSpec<AppUser>(id);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//

}