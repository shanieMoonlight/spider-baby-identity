using Shouldly;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.Teams.Qry.GetSuperTeam;

namespace ID.Application.Tests.Features.Teams.Qry.GetSuperTeam;

public class GetSuperTeamCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_ASuperMinimumValidator()
    {
        // Arrange
        var validator = new GetSuperTeamQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<GetSuperTeamQry>>();
    }

    //------------------------------------//

}//Cls