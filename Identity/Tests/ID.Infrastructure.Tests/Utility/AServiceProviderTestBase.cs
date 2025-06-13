using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public abstract class ServiceProviderTestBase
{
    protected readonly Mock<IServiceProvider> MockServiceProvider;
    protected readonly Mock<IServiceScopeFactory> MockServiceScopeFactory;
    protected readonly Mock<IServiceScope> MockServiceScope;

    protected ServiceProviderTestBase()
    {
        MockServiceProvider = new Mock<IServiceProvider>();
        MockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        MockServiceScope = new Mock<IServiceScope>();

        MockServiceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory))).Returns(MockServiceScopeFactory.Object);
        MockServiceScopeFactory.Setup(f => f.CreateScope()).Returns(MockServiceScope.Object);
        MockServiceScope.Setup(s => s.ServiceProvider).Returns(MockServiceProvider.Object);
    }
}
