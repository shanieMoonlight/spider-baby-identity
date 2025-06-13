using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class LongitudeTests
{
    [Fact]
    public void Create_WithPositiveValue_ShouldReturnThatValue()
    {
        // Arrange
        double value = 42.5;

        // Act
        var longitude = Longitude.Create(value);

        // Assert
        longitude.Value.ShouldBe(value);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        double value = 0;

        // Act
        var longitude = Longitude.Create(value);

        // Assert
        longitude.Value.ShouldBe(0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnThatValue()
    {
        // Arrange
        double value = -10.5;

        // Act
        var longitude = Longitude.Create(value);

        // Assert
        // Unlike Amount, Longitude allows negative values
        longitude.Value.ShouldBe(value);
    }

    [Theory]
    [InlineData(-180)]
    [InlineData(-90)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(90)]
    [InlineData(180)]
    public void Create_WithValidLongitudeValues_ShouldReturnTheValue(double value)
    {
        // Act
        var longitude = Longitude.Create(value);

        // Assert
        longitude.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var longitude1 = Longitude.Create(42.5);
        var longitude2 = Longitude.Create(42.5);

        // Act & Assert
        longitude1.Equals(longitude2).ShouldBeTrue();
        (longitude1 == longitude2).ShouldBeTrue();
        (longitude1 != longitude2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var longitude1 = Longitude.Create(42.5);
        var longitude2 = Longitude.Create(43.5);

        // Act & Assert
        longitude1.Equals(longitude2).ShouldBeFalse();
        (longitude1 == longitude2).ShouldBeFalse();
        (longitude1 != longitude2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var longitude1 = Longitude.Create(42.5);
        var longitude2 = Longitude.Create(42.5);

        // Act & Assert
        longitude1.GetHashCode().ShouldBe(longitude2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var longitude = Longitude.Create(42.5);

        // Act & Assert
        longitude.ToString().ShouldBe("42.5");
    }
}
