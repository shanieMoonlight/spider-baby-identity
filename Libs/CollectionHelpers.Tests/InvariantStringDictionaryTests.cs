using Shouldly;

namespace CollectionHelpers.Tests;
public class InvariantStringDictionaryTests
{
    [Fact]
    public void Keys_ShouldBeCaseInsensitive()
    {
        // Arrange
        var dict = new InvariantStringDictionary<int>
        {
            // Act
            ["Hello"] = 42
        };

        // Assert
        dict["hello"].ShouldBe(42);
        dict["HELLO"].ShouldBe(42);
        dict["HeLLo"].ShouldBe(42);
    }

    [Fact]
    public void Constructor_WithDictionary_ShouldCopyValues()
    {
        // Arrange
        var original = new Dictionary<string, string>
        {
            ["First"] = "Value1",
            ["Second"] = "Value2"
        };

        // Act
        var invariantDict = new InvariantStringDictionary<string>(original);

        // Assert
        invariantDict.Count.ShouldBe(2);
        invariantDict["first"].ShouldBe("Value1");
        invariantDict["SECOND"].ShouldBe("Value2");
    }

    [Fact]
    public void Constructor_WithKeyValuePairs_ShouldAddAll()
    {
        // Arrange
        var pairs = new[]
        {
            new KeyValuePair<string, int>("One", 1),
            new KeyValuePair<string, int>("Two", 2)
        };

        // Act
        var dict = new InvariantStringDictionary<int>(pairs);

        // Assert
        dict.Count.ShouldBe(2);
        dict["ONE"].ShouldBe(1);
        dict["two"].ShouldBe(2);
    }

    [Fact]
    public void Constructor_WithNullKeyValuePairs_ShouldNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() => new InvariantStringDictionary<int>((IEnumerable<KeyValuePair<string, int>>)null!));
    }


}//Cls
