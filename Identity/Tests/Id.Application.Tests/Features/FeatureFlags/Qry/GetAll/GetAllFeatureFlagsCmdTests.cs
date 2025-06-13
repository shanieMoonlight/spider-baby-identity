using Shouldly;
using ID.Application.Features.FeatureFlags.Cmd.Delete;
using ID.Application.Features.FeatureFlags.Qry.GetAll;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetAll;

public class GetAllFeatureFlagsCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetAllFeatureFlagsQry();

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
