using System.Linq.Expressions;
using ClArch.SimpleSpecification;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ClArch.SimpleSpecification.Tests;

public class ASimpleSpecificationTests
{
    private readonly List<TestEntity> _testData;

    public ASimpleSpecificationTests()
    {
        _testData =
        [
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true, CreatedDate = new DateTime(2023, 1, 1) },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = false, CreatedDate = new DateTime(2023, 2, 1) },
            new() { Id = 3, Name = "Charlie", Age = 35, IsActive = true, CreatedDate = new DateTime(2023, 3, 1) },
            new() { Id = 4, Name = "Diana", Age = 28, IsActive = true, CreatedDate = new DateTime(2023, 4, 1) },
            new() { Id = 5, Name = "Eve", Age = 22, IsActive = false, CreatedDate = new DateTime(2023, 5, 1) }
        ];
    }

    [Fact]
    public void Constructor_WithCriteria_ShouldSetCriteria()
    {
        // Arrange & Act
        var spec = new TestEntityByIdSpec(1);
        var criteria = spec.TESTING_GetCriteria();

        // Assert
        criteria.ShouldNotBeNull();
        var compiled = criteria.Compile();
        var testEntity = _testData.First(e => e.Id == 1);
        compiled(testEntity).ShouldBeTrue();
        
        var otherEntity = _testData.First(e => e.Id == 2);
        compiled(otherEntity).ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WithNullCriteria_ShouldSetDefaultCriteria()
    {
        // Arrange & Act
        var spec = new TestEntityWithDefaultCriteria();
        var criteria = spec.TESTING_GetCriteria();

        // Assert
        criteria.ShouldNotBeNull();
        var compiled = criteria.Compile();
        
        // Default criteria should return true for all entities
        foreach (var entity in _testData)
        {
            compiled(entity).ShouldBeTrue();
        }
    }

    [Fact]
    public void ShouldShortCircuit_WhenShortCircuitSet_ShouldReturnCorrectValue()
    {
        // Arrange
        var specWithShortCircuit = new TestEntityWithShortCircuitSpec(true);
        var specWithoutShortCircuit = new TestEntityWithShortCircuitSpec(false);

        // Act & Assert
        specWithShortCircuit.ShouldShortCircuit().ShouldBeTrue();
        specWithoutShortCircuit.ShouldShortCircuit().ShouldBeFalse();
    }

    [Fact]
    public void ShouldShortCircuit_WhenNotSet_ShouldReturnFalse()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(1);

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeFalse();
    }

    [Fact]
    public void BuildQuery_WithBasicCriteria_ShouldApplyCriteria()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityByIdSpec(1);

        // Act
        var result = spec.BuildQuery(queryable);

        // Assert
        var resultList = result.ToList();
        resultList.Count.ShouldBe(1);
        resultList[0].Id.ShouldBe(1);
        resultList[0].Name.ShouldBe("Alice");
    }

    [Fact]
    public void BuildQuery_WithAgeRangeAndOrdering_ShouldApplyCorrectly()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityByAgeRangeSpec(25, 35);

        // Act
        var result = spec.BuildQuery(queryable);

        // Assert
        var resultList = result.ToList();
        resultList.Count.ShouldBe(4); // Alice (25), Bob (30), Charlie (35), Diana (28)
        
        // Should be ordered by age
        resultList[0].Age.ShouldBe(25); // Alice
        resultList[1].Age.ShouldBe(28); // Diana
        resultList[2].Age.ShouldBe(30); // Bob
        resultList[3].Age.ShouldBe(35); // Charlie
    }

    [Fact]
    public void BuildQuery_WithPagination_ShouldApplySkipAndTake()
    {
        // Arrange
        var queryable = _testData.AsQueryable();
        var spec = new TestEntityPaginatedSpec(skip: 1, take: 2);

        // Act
        var result = spec.BuildQuery(queryable);

        // Assert
        var resultList = result.ToList();
        resultList.Count.ShouldBe(2);

        // Should be ordered by name, then skip 1, take 2
        // Criteria removes Bob and Eve
        // Ordered: Alice, Charlie, Diana
        // Skip 1 (Alice), Take 2 ( Charlie, Diana)
        resultList[0].Name.ShouldBe("Charlie");
        resultList[1].Name.ShouldBe("Diana");
    }

    [Fact]
    public void SetTake_WithZeroOrNegative_ShouldNotSetTake()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();

        // Act
        spec.SetTake(0);
        spec.SetTake(-5);

        // Assert
        var queryable = _testData.AsQueryable();
        var result = spec.BuildQuery(queryable).ToList();
        result.Count.ShouldBe(_testData.Count); // Should return all items
    }

    [Fact]
    public void SetSkip_WithZeroOrNegative_ShouldNotSetSkip()
    {
        // Arrange
        var spec = new TestEntityCustomSpec();

        // Act
        spec.SetSkip(0);
        spec.SetSkip(-5);

        // Assert
        var queryable = _testData.AsQueryable();
        var result = spec.BuildQuery(queryable).ToList();
        result.Count.ShouldBe(_testData.Count); // Should return all items from beginning
    }

    [Fact]
    public void TESTING_CompareCriteria_WithSameCriteria_ShouldReturnTrue()
    {
        // Arrange
        var spec1 = new TestEntityByIdSpec(1);
        var spec2 = new TestEntityByIdSpec(1);
        var testEntity = _testData.First();

        // Act & Assert
        spec1.TESTING_CompareCriteria(spec2, testEntity).ShouldBeTrue();
    }

    [Fact]
    public void TESTING_CompareCriteria_WithDifferentCriteria_ShouldReturnFalse()
    {
        // Arrange
        var spec1 = new TestEntityByIdSpec(1);
        var spec2 = new TestEntityByIdSpec(2);
        var testEntity = _testData.First();

        // Act & Assert
        spec1.TESTING_CompareCriteria(spec2, testEntity).ShouldBeFalse();
    }

    [Fact]
    public void TESTING_CompareCriteria_WithNullSpec_ShouldReturnFalse()
    {
        // Arrange
        var spec1 = new TestEntityByIdSpec(1);
        var testEntity = _testData.First();

        // Act & Assert
        spec1.TESTING_CompareCriteria(null!, testEntity).ShouldBeFalse();
    }
}

// Helper spec class for testing
public class TestEntityWithDefaultCriteria : ASimpleSpecification<TestEntity>
{
    public TestEntityWithDefaultCriteria() : base()
    {
    }
}

public class TestEntityCustomSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityCustomSpec() : base(e => true)
    {
    }

    public new void SetTake(int take) => base.SetTake(take);
    public new void SetSkip(int skip) => base.SetSkip(skip);
}
