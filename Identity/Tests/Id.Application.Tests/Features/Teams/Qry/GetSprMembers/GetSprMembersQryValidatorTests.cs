using ID.Application.Features.Teams.Qry.GetSprMembers;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetSprMembers;

public class GetSprMembersQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetSprMembersQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<GetSprMembersQry>>();
    }

    //------------------------------------//

}//Cls