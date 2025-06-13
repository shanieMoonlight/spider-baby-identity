using ID.Application.Features.Teams.Qry.GetMntcTeam;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetMntcTeam;

public class GetMntcTeamCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetMntcTeamQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetMntcTeamQry>>();
    }

    //------------------------------------//

}//Cls