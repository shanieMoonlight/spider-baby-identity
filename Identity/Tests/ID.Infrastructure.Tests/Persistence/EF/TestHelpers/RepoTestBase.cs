using ID.Infrastructure.Persistance.EF;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Infrastructure.Tests.Persistence.EF.TestHelpers;

/// <summary>
/// Base class for repository integration tests using in-memory database.
/// </summary>
public abstract class RepoTestBase : IDisposable
{
    protected IdDbContext DbContext { get; private set; }
    private readonly ServiceProvider _serviceProvider;

    protected RepoTestBase()
    {
        var services = new ServiceCollection();
        
        // Use in-memory database with unique name per test
        var dbName = $"TestDb_{Guid.NewGuid()}";
        services.AddDbContext<IdDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        _serviceProvider = services.BuildServiceProvider();
        DbContext = _serviceProvider.GetRequiredService<IdDbContext>();
        
        // Ensure database is created
        DbContext.Database.EnsureCreated();
    }

    //-----------------------------//

    /// <summary>
    /// Seeds the database with test data. Override in derived classes.
    /// </summary>
    protected virtual async Task SeedDatabaseAsync()
    {
        await Task.CompletedTask;
    }

    //-----------------------------//


    /// <summary>
    /// Creates a new instance of the repository under test.
    /// </summary>
    protected T CreateRepository<T>() where T : class
    {
        return (T)Activator.CreateInstance(typeof(T), DbContext)!;
    }

    //-----------------------------//


    /// <summary>
    /// Saves changes and detaches all entities to simulate fresh repository calls.
    /// </summary>
    protected async Task SaveAndDetachAllAsync()
    {
        await DbContext.SaveChangesAsync();
        
        // Detach all entities to simulate fresh repository calls
        foreach (var entry in DbContext.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }

    //-----------------------------//


    public void Dispose()
    {
        DbContext?.Dispose();
        _serviceProvider?.Dispose();
        GC.SuppressFinalize(this);
    }

    //-----------------------------//

}//Cls
