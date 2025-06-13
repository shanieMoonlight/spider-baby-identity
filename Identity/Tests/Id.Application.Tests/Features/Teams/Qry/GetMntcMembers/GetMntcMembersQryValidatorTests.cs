using ID.Application.Features.Teams.Qry.GetMntcMembers;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetMntcMembersPage;

public class GetMntcMembersQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetMntcMembersQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetMntcMembersQry>>();
    }

    //------------------------------------//

}//Cls