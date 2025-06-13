using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Utility;
using Shouldly;

namespace ClArch.ValueObjects.Tests.Utility;

public class EnsureTests
{
    #region MaxLength Tests
    [Fact]
    public void MaxLength_WithNullValue_DoesNotThrow()
    {
        // Act & Assert
        Should.NotThrow(() => Ensure.MaxLength(null, 10, "TestProperty"));
    }

    [Fact]
    public void MaxLength_WithValueUnderMaxLength_DoesNotThrow()
    {
        // Arrange
        var value = "Test";
        var maxLength = 10;

        // Act & Assert
        Should.NotThrow(() => Ensure.MaxLength(value, maxLength, "TestProperty"));
    }

    [Fact]
    public void MaxLength_WithValueEqualToMaxLength_DoesNotThrow()
    {
        // Arrange
        var value = "1234567890";
        var maxLength = 10;

        // Act & Assert
        Should.NotThrow(() => Ensure.MaxLength(value, maxLength, "TestProperty"));
    }

    [Fact]
    public void MaxLength_WithValueExceedingMaxLength_ThrowsStringTooLongPropertyException()
    {
        // Arrange
        var value = "12345678901";
        var maxLength = 10;
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<StringTooLongPropertyException>(() => 
            Ensure.MaxLength(value, maxLength, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region NotNullOrWhiteSpace Tests
    [Fact]
    public void NotNullOrWhiteSpace_WithValidValue_DoesNotThrow()
    {
        // Arrange
        var value = "Test";

        // Act & Assert
        Should.NotThrow(() => Ensure.NotNullOrWhiteSpace(value, "TestProperty"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void NotNullOrWhiteSpace_WithInvalidValue_ThrowsIsRequiredPropertyException(string? value)
    {
        // Arrange
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<IsRequiredPropertyException>(() => 
            Ensure.NotNullOrWhiteSpace(value, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region ValidId Tests
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void ValidId_WithValidValue_DoesNotThrow(int value)
    {
        // Act & Assert
        Should.NotThrow(() => Ensure.ValidId(value, "TestProperty"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidId_WithInvalidValue_ThrowsInvalidIdPropertyException(int? value)
    {
        // Arrange
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<InvalidIdPropertyException>(() => 
            Ensure.ValidId(value, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region IsRequired Tests (DateTime)
    [Fact]
    public void IsRequired_DateTime_WithValidValue_DoesNotThrow()
    {
        // Arrange
        var value = DateTime.Now;

        // Act & Assert
        Should.NotThrow(() => Ensure.IsRequired(value, "TestProperty"));
    }

    [Theory]
    [InlineData(null)]
    public void IsRequired_DateTime_WithInvalidValue_ThrowsIsRequiredPropertyException(DateTime? value)
    {
        // Arrange
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<IsRequiredPropertyException>(() => 
            Ensure.IsRequired(value, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }

    [Fact]
    public void IsNotDefault_DateTime_WithDefaultValue_ThrowsIsRequiredPropertyException()
    {
        // Arrange
        var value = default(DateTime);
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<IsRequiredPropertyException>(() => 
            Ensure.IsNotDefault<DateTime>(value, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region IsRequired Tests (Generic)
    [Fact]
    public void IsRequired_Generic_WithValidValue_DoesNotThrow()
    {
        // Arrange
        var value = new object();

        // Act & Assert
        Should.NotThrow(() => Ensure.IsRequired(value, "TestProperty"));
    }

    [Fact]
    public void IsRequired_Generic_WithNullValue_ThrowsIsRequiredPropertyException()
    {
        // Arrange
        object? value = null;
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<IsRequiredPropertyException>(() => 
            Ensure.IsRequired(value, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region ValidRange Tests
    [Fact]
    public void ValidRange_WithValueInRange_DoesNotThrow()
    {
        // Arrange
        int value = 5;
        int min = 1;
        int max = 10;

        // Act & Assert
        Should.NotThrow(() => Ensure.ValidRange(value, min, max, "TestProperty"));
    }

    [Fact]
    public void ValidRange_WithValueEqualToMin_DoesNotThrow()
    {
        // Arrange
        int value = 1;
        int min = 1;
        int max = 10;

        // Act & Assert
        Should.NotThrow(() => Ensure.ValidRange(value, min, max, "TestProperty"));
    }

    [Fact]
    public void ValidRange_WithValueEqualToMax_DoesNotThrow()
    {
        // Arrange
        int value = 10;
        int min = 1;
        int max = 10;

        // Act & Assert
        Should.NotThrow(() => Ensure.ValidRange(value, min, max, "TestProperty"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(11)]
    public void ValidRange_WithValueOutOfRange_ThrowsOutOfRangePropertyException(int value)
    {
        // Arrange
        int min = 1;
        int max = 10;
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<OutOfRangePropertyException<int>>(() => 
            Ensure.ValidRange(value, min, max, propertyName));
        
        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region MinValue Tests
    [Theory]
    [InlineData(5)]
    [InlineData(1)]
    public void MinValue_WithValidValues_DoesNotThrow(int value)
    {
        // Arrange
        int min = 1;

        // Act & Assert
        Should.NotThrow(() => Ensure.MinValue(value, min, "TestProperty"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void MinValue_WithInvalidValues_ThrowsMinValuePropertyException(int value)
    {
        // Arrange
        int min = 1;
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<MinValuePropertyException<int>>(() =>
            Ensure.MinValue(value, min, propertyName));

        exception.Property.ShouldBe(propertyName);
    }
    #endregion

    #region MaxValue Tests
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    public void MaxValue_WithValidValues_DoesNotThrow(int value)
    {
        // Arrange
        int max = 10;

        // Act & Assert
        Should.NotThrow(() => Ensure.MaxValue(value, max, "TestProperty"));
    }

    [Theory]
    [InlineData(60)]
    [InlineData(11)]
    public void MaxValue_WithInvalidValues_ThrowsMaxValuePropertyException(int value)
    {
        // Arrange
        int max = 10;
        var propertyName = "TestProperty";

        // Act & Assert
        var exception = Should.Throw<MaxValuePropertyException<int>>(() =>
            Ensure.MaxValue<int>(value, max, propertyName));

        exception.Property.ShouldBe(propertyName);
    }
    #endregion
}