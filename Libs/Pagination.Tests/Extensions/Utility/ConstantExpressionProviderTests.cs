using Pagination.Extensions.Utility;
using Shouldly;

namespace Pagination.Tests.Extensions.Utility;

public class ConstantExpressionProviderTests
{
    #region Single Value Tests

    [Theory]
    [InlineData("123", TypeCode.Int32, 123)]
    [InlineData("123.45", TypeCode.Double, 123.45)]
    [InlineData("123.45", TypeCode.Decimal, 123.45)]
    [InlineData("12345", TypeCode.Int64, 12345L)]
    [InlineData("255", TypeCode.Byte, (byte)255)]
    public void CreateNumericConstantExpression_ValidValues_ShouldCreateCorrectExpression(string input, TypeCode typeCode, object expected)
    {
        // Arrange
        var type = Type.GetType($"System.{typeCode}") ?? typeof(int);

        // Act
        var result = ConstantExpressionProvider.CreateNumericConstantExpression(type, input);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Type.ShouldBe(type);
        result.Value.Value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("abc", TypeCode.Int32)]
    [InlineData("123.45", TypeCode.Int32)]
    [InlineData("300", TypeCode.Byte)] // Byte overflow
    public void CreateNumericConstantExpression_InvalidValues_ShouldReturnFailureResult(string input, TypeCode typeCode)
    {
        // Arrange
        var type = Type.GetType($"System.{typeCode}") ?? typeof(int);

        // Act
        var result = ConstantExpressionProvider.CreateNumericConstantExpression(type, input);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain(input);
        result.Info.ShouldContain(type.Name);
        result.Value.ShouldBeNull();
    }

    #endregion

    #region List Tests

    [Fact]
    public void TryCreateNumericListConstantExpression_ValidValues_ShouldCreateCorrectList()
    {
        // Arrange
        string[] values = { "1", "2", "3", "4", "5" };
        var type = typeof(int);

        // Act
        var result = ConstantExpressionProvider.CreateNumericListConstantExpression(type, values);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        var list = result.Value.Value as List<int>;
        list.ShouldNotBeNull();
        list.Count.ShouldBe(5);
        list.ShouldBe(new List<int> { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void TryCreateNumericListConstantExpression_InvalidValue_ShouldReturnFailureResult()
    {
        // Arrange
        string[] values = { "1", "2", "not-a-number", "4", "5" };
        var type = typeof(int);

        // Act
        var result = ConstantExpressionProvider.CreateNumericListConstantExpression(type, values);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Could not parse");
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void CreateNumericListConstantExpression_NullableType_ShouldHandleCorrectly()
    {
        // Arrange
        string[] values = { "1", "2", "3" };
        var type = typeof(int?);

        // Act
        var result = ConstantExpressionProvider.CreateNumericListConstantExpression(type, values);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        // The result type should be List<int> since we're using non-nullable values
        var list = result.Value.Value as List<int>;
        list.ShouldNotBeNull();
        list.Count.ShouldBe(3);
        list.ShouldBe(new List<int> { 1, 2, 3 });
    }

    [Fact]
    public void CreateNumericConstantExpression_UnsupportedType_ShouldReturnFailureResult()
    {
        // Arrange
        var type = typeof(DateTime); // Not a numeric type

        // Act
        var result = ConstantExpressionProvider.CreateNumericConstantExpression(type, "123");

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Unsupported numeric type");
        result.Value.ShouldBeNull();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void TryCreateNumericListConstantExpression_LargeList_ShouldHandleEfficiently()
    {
        // Arrange
        const int count = 10000;
        var values = Enumerable.Range(1, count).Select(i => i.ToString()).ToArray();
        var type = typeof(int);

        // Act - measure time as a rough performance check
        var startTime = DateTime.Now;
        var result = ConstantExpressionProvider.CreateNumericListConstantExpression(type, values);
        var duration = DateTime.Now - startTime;

        // Assert
        result.Succeeded.ShouldBeTrue();

        var list = result.Value.Value as List<int>;
        list.ShouldNotBeNull();
        list.Count.ShouldBe(count);

        // We're not making specific time assertions, but this helps to compare implementations
        // and verify that the pre-sized list is working as expected
        Console.WriteLine($"Processing {count} items took {duration.TotalMilliseconds}ms");
    }

    #endregion
}
