using ID.Application.Features.Teams.Qry.GetById;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetById;

public class GetTeamByIdQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetTeamByIdQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetTeamByIdQry>>();
    }

    //------------------------------------//

}//Cls