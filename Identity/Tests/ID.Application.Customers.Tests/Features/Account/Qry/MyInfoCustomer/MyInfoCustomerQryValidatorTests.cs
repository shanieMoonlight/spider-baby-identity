using ID.Application.Customers.Features.Account.Qry.MyInfoCustomer;
using ID.Application.Customers.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Qry.MyInfoCustomer;

public class MyInfoCustomerQryValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Implements_ACustomerOnlyValidator()
    {
        // Arrange
        var validator = new MyInfoCustomerQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ACustomerOnlyValidator<MyInfoCustomerQry>>();
    }

    //------------------------------------//

}//Cls