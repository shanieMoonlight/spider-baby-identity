using ID.Application.Features.Teams.Qry.GetSprMembersPage;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetSprMembersPage;

public class GetSprMembersPageQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_ASuperMinimumValidator()
    {
        // Arrange
        var validator = new GetSprMembersPageQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<GetSprMembersPageQry>>();
    }

    //------------------------------------//

}//Cls