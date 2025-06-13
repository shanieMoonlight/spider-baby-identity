using ID.Application.Features.OutboxMessages.Qry.GetPage;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.OutboxMsgs.Qry.GetByPage;

public class GetOutboxMessagesPageQryValidatorTests
{

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetOutboxMessagePageQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetOutboxMessagePageQry>>();
    }


}//Cls