using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Qry.GetMntcTeamMember;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetMntcTeamMember;

public class GetMntcTeamMemberQryValidatorTests
{
    private readonly GetMntcTeamMemberQryValidator _validator;

    public GetMntcTeamMemberQryValidatorTests()
    {
        _validator = new GetMntcTeamMemberQryValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_MemberId_Is_Empty()
    {
        // Arrange
        var query = new GetMntcTeamMemberQry(Guid.Empty);

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
        var query = new GetMntcTeamMemberQry(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.MemberId);
    }

    //------------------------------------//

    [Fact]
    public void GetTeamMemberQryValidator_ShouldImplement_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetMntcTeamMemberQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetMntcTeamMemberQry>>();
    }

    //------------------------------------//

}//Cls
