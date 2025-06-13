using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class AmountIntegerTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        int value = 42;

        // Act
        var amount = AmountInteger.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        int value = 0;

        // Act
        var amount = AmountInteger.Create(value);

        // Assert
        amount.Value.ShouldBe(0);
        amount.Value.ShouldBe(AmountInteger.MinAmount);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinAmount()
    {
        // Arrange
        int value = -10;

        // Act
        var amount = AmountInteger.Create(value);

        // Assert
        amount.Value.ShouldBe(AmountInteger.MinAmount);
        amount.Value.ShouldBe(0);
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinAmount(int value)
    {
        // Act
        var amount = AmountInteger.Create(value);

        // Assert
        amount.Value.ShouldBe(AmountInteger.MinAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(int value)
    {
        // Act
        var amount = AmountInteger.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var amount1 = AmountInteger.Create(42);
        var amount2 = AmountInteger.Create(42);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeTrue();
        (amount1 == amount2).ShouldBeTrue();
        (amount1 != amount2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var amount1 = AmountInteger.Create(42);
        var amount2 = AmountInteger.Create(43);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeFalse();
        (amount1 == amount2).ShouldBeFalse();
        (amount1 != amount2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var amount1 = AmountInteger.Create(42);
        var amount2 = AmountInteger.Create(42);

        // Act & Assert
        amount1.GetHashCode().ShouldBe(amount2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var amount = AmountInteger.Create(42);

        // Act & Assert
        amount.ToString().ShouldBe("42");
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldCorrectValueToMinAmount()
    {
        // Arrange
        int negativeValue = -10;
        
        // Act
        var amount = AmountInteger.Create(negativeValue);
        
        // Assert
        amount.Value.ShouldNotBe(negativeValue);
        amount.Value.ShouldBe(AmountInteger.MinAmount);
    }
}
