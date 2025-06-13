using Shouldly;

namespace CollectionHelpers.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class IEnumerableExtensionsTests
{
    #region AddRange Tests

    [Fact]
    public void AddRange_IEnumerable_WithValidRanges_ConcatenatesCollections()
    {
        // Arrange
        IEnumerable<int> source = [1, 2, 3];
        IEnumerable<int> range = [4, 5, 6];

        // Act
        var result = source.AddRange(range);

        // Assert
        result.ShouldBe([1, 2, 3, 4, 5, 6]);
    }

    [Fact]
    public void AddRange_IEnumerable_WithNullSource_CreatesNewCollection()
    {
        // Arrange
        IEnumerable<int>? source = null;
        IEnumerable<int> range = [1, 2, 3];

        // Act
        var result = source.AddRange(range);

        // Assert
        result.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void AddRange_IEnumerable_WithEmptyRange_ReturnsOriginalCollection()
    {
        // Arrange
        IEnumerable<int> source = [1, 2, 3];
        IEnumerable<int> range = [];

        // Act
        var result = source.AddRange(range);

        // Assert
        result.ShouldBeSameAs(source);
    }

    [Fact]
    public void AddRange_ICollection_WithValidRanges_AddsItemsToCollection()
    {
        // Arrange
        ICollection<string> source = ["a", "b"];
        IEnumerable<string> range = ["c", "d"];

        // Act
        var result = source.AddRange(range);

        // Assert
        result.ShouldBeSameAs(source);
        result.ShouldBe(["a", "b", "c", "d"]);
    }

    [Fact]
    public void AddRange_ICollection_WithNullSource_CreatesNewCollection()
    {
        // Arrange
        ICollection<string>? source = null;
        IEnumerable<string> range = ["a", "b"];

        // Act
        var result = source.AddRange(range);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(["a", "b"]);
    }

    #endregion

    #region AnyValues Tests

    [Fact]
    public void AnyValues_List_WithItems_ReturnsTrue()
    {
        // Arrange
        List<int> list = [1, 2, 3];

        // Act & Assert
        list.AnyValues().ShouldBeTrue();
    }

    [Fact]
    public void AnyValues_List_WhenEmpty_ReturnsFalse()
    {
        // Arrange
        List<int> list = [];

        // Act & Assert
        list.AnyValues().ShouldBeFalse();
    }

    [Fact]
    public void AnyValues_List_WhenNull_ReturnsFalse()
    {
        // Arrange
        List<int>? list = null;

        // Act & Assert
        list.AnyValues().ShouldBeFalse();
    }

    [Fact]
    public void AnyValues_Array_WithItems_ReturnsTrue()
    {
        // Arrange
        int[] array = [1, 2, 3];

        // Act & Assert
        array.AnyValues().ShouldBeTrue();
    }

    [Fact]
    public void AnyValues_Array_WhenEmpty_ReturnsFalse()
    {
        // Arrange
        int[] array = [];

        // Act & Assert
        array.AnyValues().ShouldBeFalse();
    }

    [Fact]
    public void AnyValues_Array_WhenNull_ReturnsFalse()
    {
        // Arrange
        int[]? array = null;

        // Act & Assert
        array.AnyValues().ShouldBeFalse();
    }

    [Fact]
    public void AnyValues_IEnumerable_WithItems_ReturnsTrue()
    {
        // Arrange
        IEnumerable<int> enumerable = [1, 2, 3];

        // Act & Assert
        enumerable.AnyValues().ShouldBeTrue();
    }

    [Fact]
    public void AnyValues_IEnumerable_WhenEmpty_ReturnsFalse()
    {
        // Arrange
        IEnumerable<int> enumerable = [];

        // Act & Assert
        enumerable.AnyValues().ShouldBeFalse();
    }

    #endregion

    #region String Collection Tests

    [Fact]
    public void ToLower_WithStrings_ReturnsLowercaseStrings()
    {
        // Arrange
        IEnumerable<string> strings = ["Hello", "WORLD", "Test"];

        // Act
        var result = strings.ToLower();

        // Assert
        result.ShouldBe(["hello", "world", "test"]);
    }

    [Fact]
    public void ToLower_WithNullCollection_ReturnsEmptyCollection()
    {
        // Arrange
        IEnumerable<string>? strings = null;

        // Act
        var result = strings.ToLower();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToLowerList_WithStrings_ReturnsLowercaseStringList()
    {
        // Arrange
        IEnumerable<string> strings = ["Hello", "WORLD"];

        // Act
        var result = strings.ToLowerList();

        // Assert
        result.ShouldBeOfType<List<string>>();
        result.ShouldBe([ "hello", "world" ]);
    }

    #endregion

    #region String Joining Tests

    [Fact]
    public void ToSeparatedString_WithItems_JoinsWithSeparator()
    {
        // Arrange
        IEnumerable<int> items = [1, 2, 3];

        // Act
        var result = items.ToSeparatedString(";");

        // Assert
        result.ShouldBe("1;2;3");
    }

    [Fact]
    public void ToSeparatedString_WithDefaultSeparator_UsesComma()
    {
        // Arrange
        IEnumerable<string> items = ["a", "b", "c"];

        // Act
        var result = items.ToSeparatedString();

        // Assert
        result.ShouldBe("a,b,c");
    }

    [Fact]
    public void JoinStr_WithStringSelector_AppliesSelectorThenJoins()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };

        // Act
        var result = items.JoinStr("|", i => $"Item{i}");

        // Assert
        result.ShouldBe("Item1|Item2|Item3");
    }

    [Fact]
    public void JoinStr_WithIndexSelector_AppliesSelectorWithIndexThenJoins()
    {
        // Arrange
        var items = new[] { 10, 20, 30 };

        // Act
        var result = items.JoinStr('-', (i, index) => $"{index}:{i}");

        // Assert
        result.ShouldBe("0:10-1:20-2:30");
    }

    [Fact]
    public void JoinStr_WithNullSelector_UsesBasicJoin()
    {
        // Arrange
        var items = new[] { "a", "b", "c" };
        Func<string, string>? selector = null;

        // Act
        var result = items.JoinStr(",", selector);

        // Assert
        result.ShouldBe("a,b,c");
    }

    #endregion

    #region Collection Combination Tests

    [Fact]
    public void UnionSafe_WithValidCollections_CombinesUnique()
    {
        // Arrange
        IEnumerable<int> first = [1, 2, 3];
        IEnumerable<int> second = [3, 4, 5];

        // Act
        var result = first.UnionSafe(second);

        // Assert
        result.ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void UnionSafe_WithNullCollections_HandlesSafely()
    {
        // Arrange
        IEnumerable<int>? first = null;
        IEnumerable<int>? second = null;

        // Act
        var result = first.UnionSafe(second);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    #endregion

    #region Safe Conversion Tests

    [Fact]
    public void ToSafeList_WithValidCollection_ReturnsList()
    {
        // Arrange
        IEnumerable<char> source = ['a', 'b', 'c'];

        // Act
        var result = source.ToSafeList();

        // Assert
        result.ShouldBeOfType<List<char>>();
        result.ShouldBe([.. source]);
    }

    [Fact]
    public void ToSafeList_WithNullCollection_ReturnsEmptyList()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act
        var result = source.ToSafeList();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<List<int>>();
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToSafeArray_WithValidCollection_ReturnsArray()
    {
        // Arrange
        IEnumerable<double> source = [1.1, 2.2, 3.3];

        // Act
        var result = source.ToSafeArray();

        // Assert
        result.ShouldBeOfType<double[]>();
        result.ShouldBe([1.1, 2.2, 3.3]);
    }

    [Fact]
    public void ToSafeArray_WithNullCollection_ReturnsEmptyArray()
    {
        // Arrange
        IEnumerable<double>? source = null;

        // Act
        var result = source.ToSafeArray();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<double[]>();
        result.Length.ShouldBe(0);
    }

    #endregion
}