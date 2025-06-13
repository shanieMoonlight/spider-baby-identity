using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class LatitudeTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        double value = 42.5;

        // Act
        var latitude = Latitude.Create(value);

        // Assert
        latitude.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        double value = 0;

        // Act
        var latitude = Latitude.Create(value);

        // Assert
        latitude.Value.ShouldBe(0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnThatValue()
    {
        // Arrange
        double value = -10.5;

        // Act
        var latitude = Latitude.Create(value);

        // Assert
        // Unlike Amount, Latitude allows negative values
        latitude.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(-45)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(45)]
    [InlineData(90)]
    public void Create_WithValidLatitudeValues_ShouldReturnTheValue(double value)
    {
        // Act
        var latitude = Latitude.Create(value);

        // Assert
        latitude.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var latitude1 = Latitude.Create(42.5);
        var latitude2 = Latitude.Create(42.5);

        // Act & Assert
        latitude1.Equals(latitude2).ShouldBeTrue();
        (latitude1 == latitude2).ShouldBeTrue();
        (latitude1 != latitude2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var latitude1 = Latitude.Create(42.5);
        var latitude2 = Latitude.Create(43.5);

        // Act & Assert
        latitude1.Equals(latitude2).ShouldBeFalse();
        (latitude1 == latitude2).ShouldBeFalse();
        (latitude1 != latitude2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var latitude1 = Latitude.Create(42.5);
        var latitude2 = Latitude.Create(42.5);

        // Act & Assert
        latitude1.GetHashCode().ShouldBe(latitude2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var latitude = Latitude.Create(42.5);

        // Act & Assert
        latitude.ToString().ShouldBe("42.5");
    }
}
