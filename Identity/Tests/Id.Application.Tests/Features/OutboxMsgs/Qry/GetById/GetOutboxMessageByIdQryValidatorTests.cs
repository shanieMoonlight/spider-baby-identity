using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.OutboxMessages.Qry.GetById;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetById;

public class GetOutboxMessageByIdQryValidatorTests
{
    private readonly GetOutboxMessageByIdQryValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_Id_is_null()
    {
        //Arrange
        GetOutboxMessageByIdQry cmd = new(null);

        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Id);

    }


    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Act & Assert
        _validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetOutboxMessageByIdQry>>();
    }

    //------------------------------------//

}//Cls