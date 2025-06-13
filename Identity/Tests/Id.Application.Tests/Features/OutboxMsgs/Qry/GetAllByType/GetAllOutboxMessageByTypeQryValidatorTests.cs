using FluentValidation.TestHelper;
using ID.Application.Features.OutboxMessages.Qry.GetAllByType;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAllByType;

public class GetAllOutboxMessageByTypeQryValidatorTests
{
    private readonly GetAllOutboxMessagesByTypeQryValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_Id_is_null()
    {
        //Arrange
        GetAllOutboxMessagesByTypeQry cmd = new(null);

        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Type);

    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Act & Assert
        _validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetAllOutboxMessagesByTypeQry>>();
    }

    //------------------------------------//

}//Cls