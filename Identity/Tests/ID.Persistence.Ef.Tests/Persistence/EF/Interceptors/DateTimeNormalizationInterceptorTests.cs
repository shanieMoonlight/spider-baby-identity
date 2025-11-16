using ID.Persistence.Ef.Interceptors;
using Microsoft.Extensions.Logging.Abstractions;

namespace ID.Persistence.Ef.Tests.Persistence.EF.Interceptors;

public class DateTimeNormalizationInterceptorTests
{

    [Fact]
    public void BackingField_LocalDateTime_IsConvertedToUtc_OnSave()
    {
        var interceptor = new DateTimeNormalizationSaveChangesInterceptor(NullLogger<DateTimeNormalizationSaveChangesInterceptor>.Instance);

        var opts = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var local = DateTime.Now; // local

        using var ctx = new TestContext(opts, interceptor);
        ctx.BackingFieldEntities.Add(new BackingFieldEntity(local));
        ctx.SaveChanges();

        var saved = ctx.BackingFieldEntities.FirstAsync().Result;
        Assert.Equal(DateTimeKind.Utc, saved.DateCreated.Kind);
        Assert.Equal(local.ToUniversalTime().Ticks, saved.DateCreated.Ticks);
    }

    //--------------------------// 

    [Fact]
    public void AutoProperty_LocalDateTime_IsConvertedToUtc_OnSave()
    {
        var interceptor = new DateTimeNormalizationSaveChangesInterceptor(NullLogger<DateTimeNormalizationSaveChangesInterceptor>.Instance);

        var opts = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var local = DateTime.Now; // local

        using var ctx = new TestContext(opts, interceptor);
        ctx.AutoPropEntities.Add(new AutoPropEntity(local));
        ctx.SaveChanges();

        // Reload from context to simulate fresh read
        var saved = ctx.AutoPropEntities.FirstAsync().Result;
        Assert.Equal(DateTimeKind.Utc, saved.DateCreated.Kind);
        Assert.Equal(local.ToUniversalTime().Ticks, saved.DateCreated.Ticks);
    }

    //--------------------------// 

    [Fact]
    public void BackingField_UnspecifiedDateTime_TreatedAsUtc_OnSave()
    {
        var interceptor = new DateTimeNormalizationSaveChangesInterceptor(NullLogger<DateTimeNormalizationSaveChangesInterceptor>.Instance);

        var opts = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // create an Unspecified DateTime (date-only parsed by client)
        var unspecified = new DateTime(2025, 6, 21, 0, 0, 0, DateTimeKind.Unspecified);

        using var ctx = new TestContext(opts, interceptor);
        ctx.BackingFieldEntities.Add(new BackingFieldEntity(unspecified));
        ctx.SaveChanges();

        var saved = ctx.BackingFieldEntities.FirstAsync().Result;
        Assert.Equal(DateTimeKind.Utc, saved.DateCreated.Kind);
        // ticks should be preserved when treating Unspecified as UTC
        Assert.Equal(unspecified.Ticks, saved.DateCreated.Ticks);
    }

    [Fact]
    public void AutoProperty_UnspecifiedDateTime_TreatedAsUtc_OnSave()
    {
        var interceptor = new DateTimeNormalizationSaveChangesInterceptor(NullLogger<DateTimeNormalizationSaveChangesInterceptor>.Instance);

        var opts = new DbContextOptionsBuilder<TestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var unspecified = new DateTime(2025, 6, 21, 0, 0, 0, DateTimeKind.Unspecified);

        using var ctx = new TestContext(opts, interceptor);
        ctx.AutoPropEntities.Add(new AutoPropEntity(unspecified));
        ctx.SaveChanges();

        var saved = ctx.AutoPropEntities.FirstAsync()?.Result;
        Assert.Equal(DateTimeKind.Utc, saved?.DateCreated.Kind);
        Assert.Equal(unspecified.Ticks, saved?.DateCreated.Ticks);
    }

    //##########################################################//


    private class BackingFieldEntity
    {
        public int Id { get; set; }
        private DateTime _dateCreated; // explicit backing field
        public DateTime DateCreated => _dateCreated;

        protected BackingFieldEntity() { }
        public BackingFieldEntity(DateTime dt) { _dateCreated = dt; }
    }

    private class AutoPropEntity
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; private set; }

        protected AutoPropEntity() { }
        public AutoPropEntity(DateTime dt) { DateCreated = dt; }
    }

    private class TestContext(DbContextOptions options, DateTimeNormalizationSaveChangesInterceptor interceptor) : DbContext(options)
    {
        public DbSet<BackingFieldEntity> BackingFieldEntities { get; set; } = null!;
        public DbSet<AutoPropEntity> AutoPropEntities { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(interceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map the backing field explicitly so EF knows about it
            modelBuilder.Entity<BackingFieldEntity>().Property<DateTime>("DateCreated").HasField("_dateCreated");

            // AutoPropEntity: map normally
            base.OnModelCreating(modelBuilder);
        }
    }
}
