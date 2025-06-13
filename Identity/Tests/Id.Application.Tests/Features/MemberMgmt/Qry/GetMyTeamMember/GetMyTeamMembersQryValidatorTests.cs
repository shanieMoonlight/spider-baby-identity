using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Qry.GetMyTeamMember;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetTeamMember;

public class GetMyTeamMembersQryValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Should_have_error_when_MemberId_is_null()
    {
        //Arrange
        var validator = new GetMyTeamMemberQryValidator();
        GetMyTeamMemberQry cmd = new(default);

        //Act
        var result = validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.MemberId);

    }


    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new GetMyTeamMemberQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<GetMyTeamMemberQry>>();
    }

    //------------------------------------//

}//Cls