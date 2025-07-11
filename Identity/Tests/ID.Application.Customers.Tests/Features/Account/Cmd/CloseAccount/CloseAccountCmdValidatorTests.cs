﻿using FluentValidation.TestHelper;
using ID.Application.Customers.Features.Account.Cmd.CloseAccount;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.CloseAccount;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class CloseAccountCmdValidatorTests
{
    private readonly CloseAccountCmdValidator _validator;

    //- - - - - - - - - - - - -//

    public CloseAccountCmdValidatorTests()
    {
        _validator = new CloseAccountCmdValidator();
    }

    //-------------------------//

    [Fact]
    public void Should_have_error_when_TEAMID_is_empty()
    {
        //Arrange
        CloseAccountCmd cmd = new(default);


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.TeamId);

    }


    //-------------------------//


    [Fact]
    public void Implements_ACustomerOnlyValidator()
    {
        // Arrange
        var validator = new CloseAccountCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<CloseAccountCmd>>();
    }

}//Cls
