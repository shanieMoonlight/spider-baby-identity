using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class AmountIntegerNullableTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        int? value = 42;

        // Act
        var amount = AmountIntegerNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        int? value = 0;

        // Act
        var amount = AmountIntegerNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(0);
        amount.Value.ShouldBe(AmountIntegerNullable.MinAmount);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnMinAmount()
    {
        // Arrange
        int? value = -10;

        // Act
        var amount = AmountIntegerNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(AmountIntegerNullable.MinAmount);
        amount.Value.ShouldBe(0);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnNull()
    {
        // Arrange
        int? value = null;

        // Act
        var amount = AmountIntegerNullable.Create(value);

        // Assert
        amount.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-1)]
    public void Create_WithVariousNegativeValues_ShouldReturnMinAmount(int value)
    {
        // Act
        var amount = AmountIntegerNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(AmountIntegerNullable.MinAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Create_WithVariousValidValues_ShouldReturnTheValue(int value)
    {
        // Act
        var amount = AmountIntegerNullable.Create(value);

        // Assert
        amount.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var amount1 = AmountIntegerNullable.Create(42);
        var amount2 = AmountIntegerNullable.Create(42);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeTrue();
        (amount1 == amount2).ShouldBeTrue();
        (amount1 != amount2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var amount1 = AmountIntegerNullable.Create(42);
        var amount2 = AmountIntegerNullable.Create(43);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeFalse();
        (amount1 == amount2).ShouldBeFalse();
        (amount1 != amount2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var amount1 = AmountIntegerNullable.Create(null);
        var amount2 = AmountIntegerNullable.Create(null);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeTrue();
        (amount1 == amount2).ShouldBeTrue();
        (amount1 != amount2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var amount1 = AmountIntegerNullable.Create(null);
        var amount2 = AmountIntegerNullable.Create(42);

        // Act & Assert
        amount1.Equals(amount2).ShouldBeFalse();
        (amount1 == amount2).ShouldBeFalse();
        (amount1 != amount2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var amount1 = AmountIntegerNullable.Create(42);
        var amount2 = AmountIntegerNullable.Create(42);

        // Act & Assert
        amount1.GetHashCode().ShouldBe(amount2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var amount1 = AmountIntegerNullable.Create(null);
        var amount2 = AmountIntegerNullable.Create(null);

        // Act & Assert
        amount1.GetHashCode().ShouldBe(amount2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValueAsString()
    {
        // Arrange
        var amount = AmountIntegerNullable.Create(42);

        // Act & Assert
        amount.ToString().ShouldBe("42");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var amount = AmountIntegerNullable.Create(null);

        // Act & Assert
        amount.ToString().ShouldBe(string.Empty);
    }
}
