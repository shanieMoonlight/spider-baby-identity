using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class AmountTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        double value = 42.5;

        // Act
        var amount = Amount.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        double value = 0;

        // Act
        var amount = Amount.Create(value);

        // Assert
        amount.Value.ShouldBe(0);
        amount.Value.ShouldBe(Amount.MinAmount);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinAmount()
    {
        // Arrange
        double value = -10.5;

        // Act
        var amount = Amount.Create(value);

        // Assert
        amount.Value.ShouldBe(Amount.MinAmount);
        amount.Value.ShouldBe(0);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinAmount(double value)
    {
        // Act
        var amount = Amount.Create(value);

        // Assert
        amount.Value.ShouldBe(Amount.MinAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(double.MaxValue)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(double value)
    {
        // Act
        var amount = Amount.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var amount1 = Amount.Create(42.5);
        var amount2 = Amount.Create(42.5);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeTrue();
        (amount1 == amount2).ShouldBeTrue();
        (amount1 != amount2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var amount1 = Amount.Create(42.5);
        var amount2 = Amount.Create(43.5);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeFalse();
        (amount1 == amount2).ShouldBeFalse();
        (amount1 != amount2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var amount1 = Amount.Create(42.5);
        var amount2 = Amount.Create(42.5);

        // Act & Assert
        amount1.GetHashCode().ShouldBe(amount2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var amount = Amount.Create(42.5);

        // Act & Assert
        amount.ToString().ShouldBe("42.5");
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldCorrectValueToMinAmount()
    {
        // Arrange
        double negativeValue = -10;
        
        // Act
        var amount = Amount.Create(negativeValue);
        
        // Assert
        amount.Value.ShouldNotBe(negativeValue);
        amount.Value.ShouldBe(Amount.MinAmount);
    }
}
