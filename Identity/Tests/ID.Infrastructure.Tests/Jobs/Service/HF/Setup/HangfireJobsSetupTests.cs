using ID.Infrastructure.Jobs.Service.HangFire;
using ID.Infrastructure.Jobs.Service.HangFire.Background.Mgrs.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Instance.Abs;
using ID.Infrastructure.Jobs.Service.HangFire.Recurring.Mgrs.Abs;
using ID.Infrastructure.Tests.Utility;
using ID.Infrastructure.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Tests.Jobs.Service.HF.Setup;

public class HangfireJobsSetupTests
{
    [Fact]
    public void AddMyIdHangfireJobs_CallsRecurringAndBackgroundJobSetupMethods()
    {
        // Arrange
        var services = new MockServiceCollection();
        var options = SetupOptionsHelpers.CreateValidDefaultSetupOptions();
        options.ConnectionString = "Data Source=DESKTOP;Initial Catalog=MyIdDemo;Integrated Security=True;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=1;";

        // Act
        try
        {
            HangfireJobsSetup.AddMyIdHangfireJobs(services, options);
        }
        catch (Exception)
        {
            // We expect an exception when trying to create SqlServerStorage
            // The important part is that our extension methods were called
        }
        // Assert - Check for service registrations
        // This verifies that implementations of these interfaces were registered

        // Check for recurring job manager registrations
        bool hasRecurringJobMgr = services.Any(sd =>
            sd.ServiceType == typeof(IHfRecurringJobMgr) ||
            sd.ServiceType.IsAssignableTo(typeof(IHfRecurringJobMgr)));

        // Check for background job manager registrations
        bool hasBackgroundJobMgr = services.Any(sd =>
            sd.ServiceType == typeof(IHfBackgroundJobMgr) ||
            sd.ServiceType.IsAssignableTo(typeof(IHfBackgroundJobMgr)));


        bool hasIHangfireInstanceProvider = services.Any(sd =>
            sd.ServiceType == typeof(IHangfireInstanceProvider) ||
            sd.ServiceType.IsAssignableTo(typeof(IHangfireInstanceProvider)));


        bool hasHangFireJobService = services.Any(sd =>
            sd.ImplementationType == typeof(HangFireJobService) ||
            (sd.ImplementationType?.IsAssignableTo(typeof(HangFireJobService)) ?? false));


        bool hasHfStorage = services.Any(sd =>
            sd.ServiceKey?.ToString() == IdInfrastructureConstants.Jobs.DI_StorageKey);

        // Shouldly assertions
        hasRecurringJobMgr.ShouldBeTrue("Should register at least one IHfRecurringJobMgr implementation");
        hasHfStorage.ShouldBeTrue("Should register myIdHfStorage implementation");
        hasBackgroundJobMgr.ShouldBeTrue("Should register at least one IHfBackgroundJobMgr implementation");
        hasHangFireJobService.ShouldBeTrue("Should register at least one HangFireJobService implementation");
        hasIHangfireInstanceProvider.ShouldBeTrue("Should register IHangfireInstanceProvider implementation");
    }

}//Cls



//###############################################//


// Helper class to track method calls on IServiceCollection
public class MockServiceCollection : IServiceCollection
{
    private readonly IServiceCollection _inner = new ServiceCollection();
    public List<string> CalledMethods { get; } = [];

    public int Count => _inner.Count;
    public bool IsReadOnly => _inner.IsReadOnly;
    public ServiceDescriptor this[int index] { get => _inner[index]; set => _inner[index] = value; }

    public IEnumerator<ServiceDescriptor> GetEnumerator() => _inner.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _inner.GetEnumerator();
    public void Add(ServiceDescriptor item) => _inner.Add(item);
    public void Clear() => _inner.Clear();
    public bool Contains(ServiceDescriptor item) => _inner.Contains(item);
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);
    public bool Remove(ServiceDescriptor item) => _inner.Remove(item);
    public int IndexOf(ServiceDescriptor item) => _inner.IndexOf(item);
    public void Insert(int index, ServiceDescriptor item) => _inner.Insert(index, item);
    public void RemoveAt(int index) => _inner.RemoveAt(index);
}


//###############################################//