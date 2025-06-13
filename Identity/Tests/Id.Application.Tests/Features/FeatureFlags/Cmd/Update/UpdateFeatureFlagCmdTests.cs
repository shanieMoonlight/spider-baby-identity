using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Cmd.Update;
using ID.Application.Mediatr.CqrsAbs;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.Update;

public class UpdateFeatureFlagCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var dto = new FeatureFlagDto();
        var command = new UpdateFeatureFlagCmd(dto);

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
