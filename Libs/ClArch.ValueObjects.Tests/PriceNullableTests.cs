using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class PriceNullableTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        double? value = 42.5;

        // Act
        var price = PriceNullable.Create(value);

        // Assert
        price.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        double? value = 0;

        // Act
        var price = PriceNullable.Create(value);

        // Assert
        price.Value.ShouldBe(0);
        price.Value.ShouldBe(PriceNullable.MinPrice);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinPrice()
    {
        // Arrange
        double? value = -10.5;

        // Act
        var price = PriceNullable.Create(value);

        // Assert
        price.Value.ShouldBe(PriceNullable.MinPrice);
        price.Value.ShouldBe(0);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        double? value = null;

        // Act
        var price = PriceNullable.Create(value);

        // Assert
        price.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinPrice(double value)
    {
        // Act
        var price = PriceNullable.Create(value);

        // Assert
        price.Value.ShouldBe(PriceNullable.MinPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(double value)
    {
        // Act
        var price = PriceNullable.Create(value);

        // Assert
        price.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var price1 = PriceNullable.Create(42.5);
        var price2 = PriceNullable.Create(42.5);

        // Act & Assert
        price1.Equals(price2).ShouldBeTrue();
        (price1 == price2).ShouldBeTrue();
        (price1 != price2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var price1 = PriceNullable.Create(42.5);
        var price2 = PriceNullable.Create(43.5);

        // Act & Assert
        price1.Equals(price2).ShouldBeFalse();
        (price1 == price2).ShouldBeFalse();
        (price1 != price2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var price1 = PriceNullable.Create(null);
        var price2 = PriceNullable.Create(null);

        // Act & Assert
        price1.Equals(price2).ShouldBeTrue();
        (price1 == price2).ShouldBeTrue();
        (price1 != price2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var price1 = PriceNullable.Create(null);
        var price2 = PriceNullable.Create(42.5);

        // Act & Assert
        price1.Equals(price2).ShouldBeFalse();
        (price1 == price2).ShouldBeFalse();
        (price1 != price2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var price1 = PriceNullable.Create(42.5);
        var price2 = PriceNullable.Create(42.5);

        // Act & Assert
        price1.GetHashCode().ShouldBe(price2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var price1 = PriceNullable.Create(null);
        var price2 = PriceNullable.Create(null);

        // Act & Assert
        price1.GetHashCode().ShouldBe(price2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValueAsString()
    {
        // Arrange
        var price = PriceNullable.Create(42.5);

        // Act & Assert
        price.ToString().ShouldBe("42.5");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var price = PriceNullable.Create(null);

        // Act & Assert
        price.ToString().ShouldBe(string.Empty);
    }
}
