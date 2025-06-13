using ID.Application.Features.Teams.Qry.GetAll;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetAll;

public class GetAllTeamSQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetAllTeamsQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetAllTeamsQry>>();
    }

    //------------------------------------//

}//Cls