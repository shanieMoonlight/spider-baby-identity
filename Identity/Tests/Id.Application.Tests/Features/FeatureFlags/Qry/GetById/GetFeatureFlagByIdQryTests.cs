using Shouldly;
using ID.Application.Features.FeatureFlags.Qry.GetById;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetById;

public class GetFeatureFlagByIdQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetFeatureFlagByIdQry(Guid.NewGuid());


        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
