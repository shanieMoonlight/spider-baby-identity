namespace ID.API.Tests.Setup;

public class IdApiSetupExtensionsTests
{
    //[Fact]
    //public void UseMyId_CallsUseHangfireHelperAndStartRecurringMyIdJobs()
    //{
    //    // Arrange
    //    var mockApplicationBuilder = new Mock<IApplicationBuilder>();
    //    var mockServiceProvider = new Mock<IServiceProvider>();
    //    var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
    //    var mockServiceScope = new Mock<IServiceScope>();

    //    mockApplicationBuilder.Setup(app => app.ApplicationServices).Returns(mockServiceProvider.Object);
    //    mockServiceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory))).Returns(mockServiceScopeFactory.Object);
    //    mockServiceScopeFactory.Setup(ssf => ssf.CreateScope()).Returns(mockServiceScope.Object);

    //    mockApplicationBuilder.Setup(app => app.UseAuthentication()).Returns(mockApplicationBuilder.Object);
    //    mockApplicationBuilder.Setup(app => app.UseAuthorization()).Returns(mockApplicationBuilder.Object);

    //    // Act
    //    mockApplicationBuilder.Object.UseMyId();

    //    // Assert
    //    mockApplicationBuilder.Verify(app => app.UseHangfireHelper("backToSiteLink"), Times.Once);
    //    mockServiceProvider.Verify(sp => sp.StartRecurringMyIdJobs(), Times.Once);
    //}
}