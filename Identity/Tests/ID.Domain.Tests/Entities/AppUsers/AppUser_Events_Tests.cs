using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers;
using Moq;
using Shouldly;
using TestingHelpers;

namespace ID.Domain.Tests.Entities.AppUsers;

public sealed record TestDomainEvent() : IIdDomainEvent
{
    public string? UserInfo { get; set; }
}


public class AppUser_Events_Tests
{


    //------------------------------------//

    [Fact]
    public void RaiseDomainEvent_Should_Add_Event_To_DomainEvents()
    {
        // Arrange
        var appUser = new Mock<AppUser>().Object;
        var domainEvent = new Mock<IIdDomainEvent>().Object;

        // Act
        NonPublicClassMembers.InvokeMethod(appUser, "RaiseDomainEvent", [domainEvent]);

        // Assert
        var domainEvents = NonPublicClassMembers.GetField<AppUser, List<IIdDomainEvent>>(appUser, "_domainEvents");

        domainEvents!.ShouldContain(domainEvent);
    }


    //------------------------------------//

    [Fact]
    public void RaiseDomainEvent_Should_Clear_DomainEvents()
    {
        // Arrange
        var appUser = new Mock<AppUser>().Object;
        var domainEvent = new Mock<IIdDomainEvent>().Object;

        // Act
        NonPublicClassMembers.InvokeMethod(appUser, "RaiseDomainEvent", [domainEvent]);

        // Assert
        var domainEvents = NonPublicClassMembers.GetField<AppUser, List<IIdDomainEvent>>(appUser, "_domainEvents");

        // Assert 1
        domainEvents!.ShouldContain(domainEvent);


        // Act 2
        appUser.ClearDomainEvents();


        // Assert 2
        domainEvents!.ShouldBeEmpty();
    }

    //------------------------------------//

}//Cls
