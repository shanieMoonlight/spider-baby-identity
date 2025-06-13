using ID.Application.Features.OutboxMessages.Qry.GetAll;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetAll;

public class GetAllOutboxMessagesQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetAllOutboxMessagesQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetAllOutboxMessagesQry>>();
    }

    //------------------------------------//

}//Cls