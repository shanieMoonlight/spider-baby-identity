using ID.Application.AppAbs.ApplicationServices.User;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;


namespace Id.Application.Tests.Utility.Mocks;
internal class MockFindUserService
{


    public static Mock<IFindUserService<AppUser>> GetSuccessfulService(AppUser? returnedUser = null)
    {
        returnedUser ??= AppUserDataFactory.Create();
        var mock = new Mock<IFindUserService<AppUser>>();


        mock.Setup(r => r.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
          .ReturnsAsync(returnedUser);


        mock.Setup(r => r.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
          .ReturnsAsync(returnedUser);     
        

        return mock;

    }

    //-----------------------------//

    public static Mock<IFindUserService<AppUser>> GetFailureService()
    {

        var mock = new Mock<IFindUserService<AppUser>>();


        mock.Setup(r => r.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
          .ReturnsAsync((string? email, string? username, Guid? userId) => null);

        mock.Setup(r => r.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
          .ReturnsAsync((Guid? userId) => null);

        return mock;

    }

    //-----------------------------//


}
