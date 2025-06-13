using Shouldly;
using ID.Application.Features.FeatureFlags.Qry.GetByName;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetByName;

public class GetFeatureFlagByNameQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetFeatureFlagByNameQry("name");

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
