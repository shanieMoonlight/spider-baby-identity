using ClArch.SimpleSpecification;
using Shouldly;

namespace ClArch.SimpleSpecification.Tests;

public class SpecificationBuilderTests
{
    private readonly List<TestEntity> _testData;

    public SpecificationBuilderTests()
    {
        _testData =
        [
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = false },
            new() { Id = 3, Name = "Charlie", Age = 35, IsActive = true }
        ];
    }

    [Fact]
    public void BuildQuery_ShouldReturnFilteredResults()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityByIdSpec(1);

        // Act
        var result = queryable.BuildQuery(spec);

        // Assert
        var resultList = result.ToList();
        resultList.Count.ShouldBe(1);
        resultList[0].Id.ShouldBe(1);
        resultList[0].Name.ShouldBe("Alice");
    }

    [Fact]
    public void BuildQuery_WithComplexSpec_ShouldApplyAllFilters()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityByAgeRangeSpec(25, 35);

        // Act
        var result = queryable.BuildQuery(spec);

        // Assert
        var resultList = result.ToList();
        resultList.Count.ShouldBe(3);
        
        // Should be ordered by age
        resultList[0].Age.ShouldBe(25);
        resultList[1].Age.ShouldBe(30);
        resultList[2].Age.ShouldBe(35);
    }

    [Fact]
    public void BuildQuery_WithPaginatedSpec_ShouldApplyPagination()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityPaginatedSpec(skip: 1, take: 1);

        // Act
        var result = queryable.BuildQuery(spec);

        // Assert
        var resultList = result.ToList();
        resultList.Count.ShouldBe(1);
        // Should skip first active entity (Alice) and take next one (Charlie)
        resultList[0].Name.ShouldBe("Charlie");
    }

    [Fact]
    public void BuildQuery_WithNoMatchingResults_ShouldReturnEmpty()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityByIdSpec(999); // Non-existent ID

        // Act
        var result = queryable.BuildQuery(spec);

        // Assert
        result.ToList().ShouldBeEmpty();
    }

    [Fact]
    public void BuildQuery_WithEmptyQueryable_ShouldReturnEmpty()
    {
        // Arrange
        var emptyQueryable = new List<TestEntity>().AsQueryable();
        var spec = new TestEntityByIdSpec(1);

        // Act
        var result = emptyQueryable.BuildQuery(spec);

        // Assert
        result.ToList().ShouldBeEmpty();
    }
}
