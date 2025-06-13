using ID.Application.Customers.Mediatr.Validation;

namespace ID.Application.Tests.Mediatr.Validation.CustomerValidators.Data;

internal class TestCustomerMinimumValidator : ACustomerMinimumValidator<TestRequest>
{
    public TestCustomerMinimumValidator() : base() { }
    public TestCustomerMinimumValidator(int position) : base(position) { }
}