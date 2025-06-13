using System;
using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Qry.GetTeamMemberQry;
using ID.Application.Mediatr.Validation;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using Xunit;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetTeamMember;

public class GetTeamMemberQryValidatorTests
{
    private readonly GetTeamMemberQryValidator _validator;

    public GetTeamMemberQryValidatorTests()
    {
        _validator = new GetTeamMemberQryValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_MemberId_Is_Empty()
    {
        // Arrange
        var query = new GetTeamMemberQry(Guid.Empty, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.TeamId);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_TeamId_Is_Empty()
    {
        // Arrange
        var query = new GetTeamMemberQry(Guid.NewGuid(), Guid.Empty);

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
        var query = new GetTeamMemberQry(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.MemberId);
        result.ShouldNotHaveValidationErrorFor(q => q.TeamId);
    }

    //------------------------------------//

    [Fact]
    public void GetTeamMemberQryValidator_ShouldImplement_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetTeamMemberQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetTeamMemberQry>>();
    }

    //------------------------------------//

}//Cls
