using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class PriceIntegerTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        int value = 42;

        // Act
        var price = PriceInteger.Create(value);

        // Assert
        price.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        int value = 0;

        // Act
        var price = PriceInteger.Create(value);

        // Assert
        price.Value.ShouldBe(0);
        price.Value.ShouldBe(PriceInteger.MinPrice);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinPrice()
    {
        // Arrange
        int value = -10;

        // Act
        var price = PriceInteger.Create(value);

        // Assert
        price.Value.ShouldBe(PriceInteger.MinPrice);
        price.Value.ShouldBe(0);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinPrice(int value)
    {
        // Act
        var price = PriceInteger.Create(value);

        // Assert
        price.Value.ShouldBe(PriceInteger.MinPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(int value)
    {
        // Act
        var price = PriceInteger.Create(value);

        // Assert
        price.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var price1 = PriceInteger.Create(42);
        var price2 = PriceInteger.Create(42);

        // Act & Assert
        price1.Equals(price2).ShouldBeTrue();
        (price1 == price2).ShouldBeTrue();
        (price1 != price2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var price1 = PriceInteger.Create(42);
        var price2 = PriceInteger.Create(43);

        // Act & Assert
        price1.Equals(price2).ShouldBeFalse();
        (price1 == price2).ShouldBeFalse();
        (price1 != price2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var price1 = PriceInteger.Create(42);
        var price2 = PriceInteger.Create(42);

        // Act & Assert
        price1.GetHashCode().ShouldBe(price2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var price = PriceInteger.Create(42);

        // Act & Assert
        price.ToString().ShouldBe("42");
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldCorrectValueToMinPrice()
    {
        // Arrange
        int negativeValue = -10;
        
        // Act
        var price = PriceInteger.Create(negativeValue);
        
        // Assert
        price.Value.ShouldNotBe(negativeValue);
        price.Value.ShouldBe(PriceInteger.MinPrice);
    }
}
