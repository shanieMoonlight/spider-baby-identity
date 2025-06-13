using ID.Application.Features.Teams.Qry.GetByName;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetByName;

public class GetByNameQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetTeamsByNameQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetTeamsByNameQry>>();
    }

    //------------------------------------//

}//Cls