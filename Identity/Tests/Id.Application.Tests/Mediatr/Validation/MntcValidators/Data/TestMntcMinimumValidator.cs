using ID.Application.Mediatr.Validation;

namespace ID.Application.Tests.Mediatr.Validation.MntcValidators.Data;

internal class TestMntcMinimumValidator : AMntcMinimumValidator<TestRequest>
{
    public TestMntcMinimumValidator() : base() { }
    public TestMntcMinimumValidator(int position) : base(position) { }
}