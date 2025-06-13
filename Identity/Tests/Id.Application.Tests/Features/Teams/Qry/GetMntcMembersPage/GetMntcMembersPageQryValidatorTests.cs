using ID.Application.Features.Teams.Qry.GetMntcMembersPage;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetMntcMembersPage;

public class GetMntcMembersPageQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetMntcMembersPageQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetMntcMembersPageQry>>();
    }

    //------------------------------------//

}//Cls