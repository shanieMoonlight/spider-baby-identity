using ID.Application.Features.Account.Qry.MyInfo;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Qry.MyInfo;

public class MyInfoQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new MyInfoQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<MyInfoQry>>();
    }

    //------------------------------------//

}//Cls