using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class PriceIntegerNullableTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        int? value = 42;

        // Act
        var price = PriceIntegerNullable.Create(value);

        // Assert
        price.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        int? value = 0;

        // Act
        var price = PriceIntegerNullable.Create(value);

        // Assert
        price.Value.ShouldBe(0);
        price.Value.ShouldBe(PriceIntegerNullable.MinPrice);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinPrice()
    {
        // Arrange
        int? value = -10;

        // Act
        var price = PriceIntegerNullable.Create(value);

        // Assert
        price.Value.ShouldBe(PriceIntegerNullable.MinPrice);
        price.Value.ShouldBe(0);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        int? value = null;

        // Act
        var price = PriceIntegerNullable.Create(value);

        // Assert
        price.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinPrice(int value)
    {
        // Act
        var price = PriceIntegerNullable.Create(value);

        // Assert
        price.Value.ShouldBe(PriceIntegerNullable.MinPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(int value)
    {
        // Act
        var price = PriceIntegerNullable.Create(value);

        // Assert
        price.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var price1 = PriceIntegerNullable.Create(42);
        var price2 = PriceIntegerNullable.Create(42);

        // Act & Assert
        price1.Equals(price2).ShouldBeTrue();
        (price1 == price2).ShouldBeTrue();
        (price1 != price2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var price1 = PriceIntegerNullable.Create(42);
        var price2 = PriceIntegerNullable.Create(43);

        // Act & Assert
        price1.Equals(price2).ShouldBeFalse();
        (price1 == price2).ShouldBeFalse();
        (price1 != price2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var price1 = PriceIntegerNullable.Create(null);
        var price2 = PriceIntegerNullable.Create(null);

        // Act & Assert
        price1.Equals(price2).ShouldBeTrue();
        (price1 == price2).ShouldBeTrue();
        (price1 != price2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var price1 = PriceIntegerNullable.Create(null);
        var price2 = PriceIntegerNullable.Create(42);

        // Act & Assert
        price1.Equals(price2).ShouldBeFalse();
        (price1 == price2).ShouldBeFalse();
        (price1 != price2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var price1 = PriceIntegerNullable.Create(42);
        var price2 = PriceIntegerNullable.Create(42);

        // Act & Assert
        price1.GetHashCode().ShouldBe(price2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var price1 = PriceIntegerNullable.Create(null);
        var price2 = PriceIntegerNullable.Create(null);

        // Act & Assert
        price1.GetHashCode().ShouldBe(price2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValueAsString()
    {
        // Arrange
        var price = PriceIntegerNullable.Create(42);

        // Act & Assert
        price.ToString().ShouldBe("42");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var price = PriceIntegerNullable.Create(null);

        // Act & Assert
        price.ToString().ShouldBe(string.Empty);
    }
}
