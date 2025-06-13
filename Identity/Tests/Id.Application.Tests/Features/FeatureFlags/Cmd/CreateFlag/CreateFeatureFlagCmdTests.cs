using Shouldly;
using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Cmd.Create;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.CreateFlag;

public class CreateFeatureFlagCmdTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var dto = new FeatureFlagDto();
        var command = new CreateFeatureFlagCmd(dto);

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
