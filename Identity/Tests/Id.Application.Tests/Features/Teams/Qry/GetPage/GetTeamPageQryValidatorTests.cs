using ID.Application.Features.Teams.Qry.GetPage;
using ID.Application.Mediatr.Validation;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetPage;

public class GetTeamPageQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_ASuperMinimumValidator()
    {
        // Arrange
        var mockEnv = new Mock<IWebHostEnvironment>();
        var validator = new GetTeamPageQryValidator(mockEnv.Object);

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumOrDevValidator<GetTeamsPageQry>>();
    }

    //------------------------------------//

}//Cls
