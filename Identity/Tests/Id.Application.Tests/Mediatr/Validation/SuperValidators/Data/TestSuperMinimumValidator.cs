using ID.Application.Mediatr.Validation;

namespace ID.Application.Tests.Mediatr.Validation.SuperValidators.Data;

internal class TestSuperMinimumValidator : ASuperMinimumValidator<TestRequest>
{
    public TestSuperMinimumValidator() : base() { }
    public TestSuperMinimumValidator(int position) : base(position) { }
}