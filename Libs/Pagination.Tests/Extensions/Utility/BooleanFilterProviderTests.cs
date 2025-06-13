using Pagination.Extensions.Utility;
using Pagination.Utility;
using Shouldly;
using System.Linq.Expressions;

namespace Pagination.Tests.Extensions.Utility;

public class BooleanFilterProviderTests
{
    private class TestEntity
    {
        public bool IsActive { get; set; }
    }

    private readonly ParameterExpression _param = Expression.Parameter(typeof(TestEntity), "x");

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("TRUE", true)]
    [InlineData("false", false)]
    [InlineData("False", false)]
    [InlineData("FALSE", false)]
    public void CreateBooleanFilter_ValidBooleanValues_ShouldCreateCorrectExpression(string value, bool expected)
    {
        // Arrange
        var filterRequest = new FilterRequest
        {
            Field = "IsActive",
            FilterValue = value,
            FilterType = FilterTypes.EQUALS,
            FilterDataType = FilterDataTypes.BOOLEAN
        };

        // Act
        var result = BooleanFilterProvider.CreateBooleanFilter<TestEntity>(_param, filterRequest);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        // Compile and test the expression
        var lambda = Expression.Lambda<Func<TestEntity, bool>>(result.Value, _param);
        var func = lambda.Compile();

        // Test with both matching and non-matching values
        var matchingEntity = new TestEntity { IsActive = expected };
        var nonMatchingEntity = new TestEntity { IsActive = !expected };

        func(matchingEntity).ShouldBeTrue();
        func(nonMatchingEntity).ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("1")]
    [InlineData("0")]
    public void CreateBooleanFilter_InvalidBooleanValues_ShouldReturnErrorResult(string value)
    {
        // Arrange
        var filterRequest = new FilterRequest
        {
            Field = "IsActive",
            FilterValue = value,
            FilterType = FilterTypes.EQUALS,
            FilterDataType = FilterDataTypes.BOOLEAN
        };

        // Act
        var result = BooleanFilterProvider.CreateBooleanFilter<TestEntity>(_param, filterRequest);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Can't convert");
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void CreateBooleanFilter_AllValue_ShouldReturnAppropriateResult()
    {
        // Arrange
        var filterRequest = new FilterRequest
        {
            Field = "IsActive",
            FilterValue = BooleanFilterValues.ALL,
            FilterType = FilterTypes.EQUALS,
            FilterDataType = FilterDataTypes.BOOLEAN
        };

        // Act
        var result = BooleanFilterProvider.CreateBooleanFilter<TestEntity>(_param, filterRequest);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("No need to filter");
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void CreateBooleanFilter_WithPropertySelector_ShouldUseTransformedPropertyName()
    {
        // Arrange
        var filterRequest = new FilterRequest
        {
            Field = "active", // lowercase field name that will be transformed
            FilterValue = "true",
            FilterType = FilterTypes.EQUALS,
            FilterDataType = FilterDataTypes.BOOLEAN
        };

        // Custom property selector that transforms field names
        static string propertySelector(string fieldName) =>
            fieldName.Equals("active", StringComparison.OrdinalIgnoreCase) ? "IsActive" : fieldName;

        // Act
        var result = BooleanFilterProvider.CreateBooleanFilter<TestEntity>(_param, filterRequest, propertySelector);

        // Assert
        result.Succeeded.ShouldBeTrue();

        // Compile and test the expression
        var lambda = Expression.Lambda<Func<TestEntity, bool>>(result.Value, _param);
        var func = lambda.Compile();

        // Should work with the transformed property name
        var entity = new TestEntity { IsActive = true };
        func(entity).ShouldBeTrue();
    }
}
