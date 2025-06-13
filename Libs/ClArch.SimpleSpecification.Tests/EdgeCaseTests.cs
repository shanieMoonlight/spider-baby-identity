using ClArch.SimpleSpecification;
using Shouldly;

namespace ClArch.SimpleSpecification.Tests;

public class EdgeCaseTests
{
    [Fact]
    public void SetTake_WithNegativeValue_ShouldNotSetTake()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true }
        };

        // Act
        spec.SetTake(-5);
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(2); // Should return all items
    }

    [Fact]
    public void SetTake_WithZero_ShouldNotSetTake()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true }
        };

        // Act
        spec.SetTake(0);
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(2); // Should return all items
    }

    [Fact]
    public void SetSkip_WithNegativeValue_ShouldNotSetSkip()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true }
        };

        // Act
        spec.SetSkip(-5);
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(2); // Should start from beginning
        result[0].Name.ShouldBe("Alice");
    }

    [Fact]
    public void SetSkip_WithZero_ShouldNotSetSkip()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true }
        };

        // Act
        spec.SetSkip(0);
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(2); // Should start from beginning
        result[0].Name.ShouldBe("Alice");
    }

    [Fact]
    public void BuildQuery_WithEmptyQueryable_ShouldReturnEmpty()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(1);
        var emptyData = new List<TestEntity>().AsQueryable();

        // Act
        var result = spec.BuildQuery(emptyData).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void BuildQuery_WithNullCriteria_ShouldReturnAll()
    {
        // Arrange
        var spec = new TestEntityWithDefaultCriteria();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = false },
            new() { Id = 3, Name = "Charlie", Age = 35, IsActive = true }
        };

        // Act
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(3); // Should return all items
    }

    [Fact]
    public void SetTake_WithLargeValue_ShouldWork()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true }
        };

        // Act
        spec.SetTake(1000); // Much larger than available data
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(2); // Should return all available items
    }

    [Fact]
    public void SetSkip_WithLargeValue_ShouldReturnEmpty()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true }
        };

        // Act
        spec.SetSkip(1000); // Much larger than available data
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void TESTING_CompareCriteria_WithSameEntityDifferentValues_ShouldWork()
    {
        // Arrange
        var spec1 = new TestEntityByIdSpec(1);
        var spec2 = new TestEntityByIdSpec(2);        
        var entity1 = new TestEntity { Id = 1, Name = "Alice", Age = 25, IsActive = true };
        var entity2 = new TestEntity { Id = 2, Name = "Bob", Age = 30, IsActive = false };

        // Act & Assert
        spec1.TESTING_CompareCriteria(spec1, entity1).ShouldBeTrue(); // Same spec: spec1(entity1) == spec1(entity1) → true == true → true
        spec1.TESTING_CompareCriteria(spec1, entity2).ShouldBeTrue(); // Same spec: spec1(entity2) == spec1(entity2) → false == false → true  
        spec1.TESTING_CompareCriteria(spec2, entity1).ShouldBeFalse(); // Different specs: spec1(entity1)=true, spec2(entity1)=false → true != false
        spec1.TESTING_CompareCriteria(spec2, entity2).ShouldBeFalse(); // Different specs: spec1(entity2)=false, spec2(entity2)=true → false != true
    }

    [Fact]
    public void BuildQuery_MultipleChainingOperations_ShouldWork()
    {
        // Arrange
        var spec = new TestEntityComplexSpec();
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = true },
            new() { Id = 3, Name = "Charlie", Age = 35, IsActive = true },
            new() { Id = 4, Name = "Diana", Age = 40, IsActive = true },
            new() { Id = 5, Name = "Eve", Age = 20, IsActive = false }
        };

        // Act
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        result.Count.ShouldBe(2); // Skip 1, Take 2 from active entities ordered by age
        result[0].Age.ShouldBe(30); // Bob (after skipping Alice)
        result[1].Age.ShouldBe(35); // Charlie
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(0, 3)]
    [InlineData(2, 1)]
    [InlineData(10, 5)]
    public void SetTakeAndSkip_WithVariousValues_ShouldWork(int skip, int take)
    {
        // Arrange
        var spec = new TestEntityCustomSpec();
        var testData = Enumerable.Range(1, 10)
            .Select(i => new TestEntity { Id = i, Name = $"Entity{i}", Age = 20 + i, IsActive = true })
            .ToList();

        // Act
        spec.SetSkip(skip);
        spec.SetTake(take);
        var result = spec.BuildQuery(testData.AsQueryable()).ToList();

        // Assert
        var expectedCount = Math.Max(0, Math.Min(take, testData.Count - skip));
        result.Count.ShouldBe(expectedCount);

        if (result.Any())
        {
            result[0].Id.ShouldBe(skip + 1); // First item should be after skip
        }
    }
}

// Complex specification for testing multiple operations
public class TestEntityComplexSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityComplexSpec() : base(e => e.IsActive && e.Age >= 25)
    {
        SetOrderBy(q => q.OrderBy(e => e.Age));
        SetSkip(1);
        SetTake(2);
    }
}
