using ClArch.SimpleSpecification;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ClArch.SimpleSpecification.Tests;

// Test DbContext for integration tests
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<TestChildEntity> TestChildEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasMany(e => e.Children)
                  .WithOne(c => c.Parent)
                  .HasForeignKey(c => c.ParentId);
        });

        modelBuilder.Entity<TestChildEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(200);
        });
    }
}

public class IntegrationTests : IDisposable
{
    private readonly TestDbContext _context;

    public IntegrationTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var entities = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true, CreatedDate = new DateTime(2023, 1, 1) },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = false, CreatedDate = new DateTime(2023, 2, 1) },
            new() { Id = 3, Name = "Charlie", Age = 35, IsActive = true, CreatedDate = new DateTime(2023, 3, 1) },
            new() { Id = 4, Name = "Diana", Age = 28, IsActive = true, CreatedDate = new DateTime(2023, 4, 1) },
            new() { Id = 5, Name = "Eve", Age = 22, IsActive = false, CreatedDate = new DateTime(2023, 5, 1) }
        };

        var children = new List<TestChildEntity>
        {
            new() { Id = 1, Description = "Alice's first child", ParentId = 1 },
            new() { Id = 2, Description = "Alice's second child", ParentId = 1 },
            new() { Id = 3, Description = "Charlie's child", ParentId = 3 },
        };

        _context.TestEntities.AddRange(entities);
        _context.TestChildEntities.AddRange(children);
        _context.SaveChanges();
    }

    [Fact]
    public async Task BuildQuery_WithEFCore_ShouldExecuteCorrectly()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(1);

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .ToListAsync();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(1);
        result[0].Name.ShouldBe("Alice");
    }

    [Fact]
    public async Task BuildQuery_WithComplexQuery_ShouldExecuteCorrectly()
    {
        // Arrange
        var spec = new TestEntityByAgeRangeSpec(25, 35);

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .ToListAsync();

        // Assert
        result.Count.ShouldBe(4);
        // Should be ordered by age
        result[0].Age.ShouldBe(25); // Alice
        result[1].Age.ShouldBe(28); // Diana
        result[2].Age.ShouldBe(30); // Bob
        result[3].Age.ShouldBe(35); // Charlie
    }

    [Fact]
    public async Task BuildQuery_WithIncludes_ShouldLoadNavigationProperties()
    {
        // Arrange
        var spec = new TestEntityWithChildrenSpec();

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .ToListAsync();

        // Assert
        result.Count.ShouldBe(3); // Only active entities (Alice, Charlie, Diana)
        
        // Find Alice - she should have children loaded
        var alice = result.FirstOrDefault(e => e.Name == "Alice");
        alice.ShouldNotBeNull();
        alice.Children.ShouldNotBeNull();
        alice.Children.Count.ShouldBe(2);
        
        // Find Charlie - he should have children loaded
        var charlie = result.FirstOrDefault(e => e.Name == "Charlie");
        charlie.ShouldNotBeNull();
        charlie.Children.ShouldNotBeNull();
        charlie.Children.Count.ShouldBe(1);
    }

    [Fact]
    public async Task BuildQuery_WithPagination_ShouldLimitResults()
    {
        // Arrange
        var spec = new TestEntityPaginatedSpec(skip: 1, take: 2);

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .ToListAsync();

        // Assert
        result.Count.ShouldBe(2);
        // Active entities ordered by name: Alice, Charlie, Diana
        // Skip 1 (Alice), Take 2 (Charlie, Diana)
        result[0].Name.ShouldBe("Charlie");
        result[1].Name.ShouldBe("Diana");
    }

    [Fact]
    public async Task BuildQuery_WithNoResults_ShouldReturnEmpty()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(999); // Non-existent ID

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .ToListAsync();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithSpec_ShouldReturnSingleResult()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(1);

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
        result.Name.ShouldBe("Alice");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithNoResults_ShouldReturnNull()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(999);

        // Act
        var result = await _context.TestEntities
            .BuildQuery(spec)
            .FirstOrDefaultAsync();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task CountAsync_WithSpec_ShouldReturnCorrectCount()
    {
        // Arrange
        var spec = new TestEntityByAgeRangeSpec(25, 35);

        // Act
        var count = await _context.TestEntities
            .BuildQuery(spec)
            .CountAsync();

        // Assert
        count.ShouldBe(4);
    }

    [Fact]
    public async Task AnyAsync_WithSpec_ShouldReturnCorrectResult()
    {
        // Arrange
        var specWithResults = new TestEntityByIdSpec(1);
        var specWithoutResults = new TestEntityByIdSpec(999);

        // Act
        var hasResults = await _context.TestEntities
            .BuildQuery(specWithResults)
            .AnyAsync();
            
        var hasNoResults = await _context.TestEntities
            .BuildQuery(specWithoutResults)
            .AnyAsync();

        // Assert
        hasResults.ShouldBeTrue();
        hasNoResults.ShouldBeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
