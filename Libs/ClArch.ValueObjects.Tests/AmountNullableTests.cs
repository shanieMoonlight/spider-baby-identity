using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class AmountNullableTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        double? value = 42.5;

        // Act
        var amount = AmountNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        double? value = 0;

        // Act
        var amount = AmountNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(0);
        amount.Value.ShouldBe(AmountNullable.MinAmount);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinAmount()
    {
        // Arrange
        double? value = -10.5;

        // Act
        var amount = AmountNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(AmountNullable.MinAmount);
        amount.Value.ShouldBe(0);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        double? value = null;

        // Act
        var amount = AmountNullable.Create(value);

        // Assert
        amount.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinAmount(double value)
    {
        // Act
        var amount = AmountNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(AmountNullable.MinAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(double value)
    {
        // Act
        var amount = AmountNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var amount1 = AmountNullable.Create(42.5);
        var amount2 = AmountNullable.Create(42.5);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeTrue();
        (amount1 == amount2).ShouldBeTrue();
        (amount1 != amount2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var amount1 = AmountNullable.Create(42.5);
        var amount2 = AmountNullable.Create(43.5);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeFalse();
        (amount1 == amount2).ShouldBeFalse();
        (amount1 != amount2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var amount1 = AmountNullable.Create(null);
        var amount2 = AmountNullable.Create(null);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeTrue();
        (amount1 == amount2).ShouldBeTrue();
        (amount1 != amount2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var amount1 = AmountNullable.Create(null);
        var amount2 = AmountNullable.Create(42.5);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeFalse();
        (amount1 == amount2).ShouldBeFalse();
        (amount1 != amount2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var amount1 = AmountNullable.Create(42.5);
        var amount2 = AmountNullable.Create(42.5);

        // Act & Assert
        amount1.GetHashCode().ShouldBe(amount2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var amount1 = AmountNullable.Create(null);
        var amount2 = AmountNullable.Create(null);

        // Act & Assert
        amount1.GetHashCode().ShouldBe(amount2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValueAsString()
    {
        // Arrange
        var amount = AmountNullable.Create(42.5);

        // Act & Assert
        amount.ToString().ShouldBe("42.5");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var amount = AmountNullable.Create(null);

        // Act & Assert
        amount.ToString().ShouldBe(string.Empty);
    }
}
