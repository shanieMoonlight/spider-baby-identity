using Shouldly;
using ID.Application.Features.FeatureFlags.Qry.GetPage;
using Pagination;
using ID.Application.Mediatr.CqrsAbs;

namespace ID.Application.Tests.Features.FeatureFlags.Qry.GetByPage;

public class GetByNameQryTests
{
    [Fact]
    public void AddMntcMemberCmd_Implements_IPrincipalInfoRequest()
    {
        // Arrange
        var command = new GetFeatureFlagsPageQry(PagedRequest.Empty());

        // Act & Assert
        Assert.IsType<IIdPrincipalInfoRequest>(command, exactMatch: false);

        // Act & Assert
        command.ShouldBeAssignableTo<IIdPrincipalInfoRequest>();
    }
}
