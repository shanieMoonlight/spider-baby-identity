using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Qry.GetSuperTeamMember;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetSuperTeamMember;

public class GetSuperTeamMemberQryValidatorTests
{
    private readonly GetSuperTeamMemberQryValidator _validator;

    public GetSuperTeamMemberQryValidatorTests()
    {
        _validator = new GetSuperTeamMemberQryValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_MemberId_Is_Empty()
    {
        // Arrange
        var query = new GetSuperTeamMemberQry(Guid.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.MemberId);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Ids_Are_Valid()
    {
        // Arrange
        var query = new GetSuperTeamMemberQry(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.MemberId);
    }

    //------------------------------------//

    [Fact]
    public void GetTeamMemberQryValidator_ShouldImplement_ASuperMinimumValidator()
    {
        // Arrange
        var validator = new GetSuperTeamMemberQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<GetSuperTeamMemberQry>>();
    }

    //------------------------------------//

}//Cls
