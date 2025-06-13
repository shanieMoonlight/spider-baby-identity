using ID.Application.Features.FeatureFlags.Cmd.Delete;
using ID.Application.Mediatr.CqrsAbs;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.DeleteFlag;

public class DeleteFeatureFlagCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new DeleteFeatureFlagCmd(Guid.NewGuid());

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
