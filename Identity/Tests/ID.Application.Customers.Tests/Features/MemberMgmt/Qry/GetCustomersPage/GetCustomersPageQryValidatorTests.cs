using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomersPage;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Qry.GetCustomersPage;

public class GetCustomersPageQryValidatorTests
{

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new GetCustomersPageQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<GetCustomersPageQry>>();
    }

    //------------------------------------//

}//Cls