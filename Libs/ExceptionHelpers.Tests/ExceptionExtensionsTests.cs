using ExceptionHelpers;
using Shouldly;

namespace ExceptionExtensions.Tests;

public class ExceptionExtensionsTests
{
    [Fact]
    public void ToLogString_BasicException_ContainsAllExpectedFields()
    {
        // Arrange
        var ex = new InvalidOperationException("Test exception");

        // Act
        var result = ex.ToLogString();

        // Assert
        result.ShouldContain("InvalidOperationException");
        result.ShouldContain("Test exception");
        result.ShouldContain("TimeStamp:");
        result.ShouldContain("Stack Trace:");
    }

    [Fact]
    public void ToLogString_WithInnerException_IncludesBothExceptions()
    {
        // Arrange
        var innerEx = new ArgumentException("Inner message");
        var outerEx = new InvalidOperationException("Outer message", innerEx);

        // Act
        var result = outerEx.ToLogString();

        // Assert
        result.ShouldContain("InvalidOperationException");
        result.ShouldContain("Outer message");
        result.ShouldContain("ArgumentException");
        result.ShouldContain("Inner message");
    }

    [Fact]
    public void ToLogString_WithAggregateException_FormatsMultipleExceptions()
    {
        // Arrange
        var exceptions = new List<Exception>
        {
            new ArgumentException("First error"),
            new NullReferenceException("Second error")
        };
        var aggregateEx = new AggregateException("Multiple errors", exceptions);

        // Act
        var result = aggregateEx.ToLogString();

        // Assert
        result.ShouldContain("Aggregate Exception with multiple inner exceptions");
        result.ShouldContain("Inner Exception #1");
        result.ShouldContain("Inner Exception #2");
        result.ShouldContain("ArgumentException");
        result.ShouldContain("NullReferenceException");
    }

    [Fact]
    public void ToLogString_WithExceptionData_IncludesDataEntries()
    {
        // Arrange
        var ex = new Exception("Exception with data");
        ex.Data["CustomKey1"] = "Value1";
        ex.Data["CustomKey2"] = 42;

        // Act
        var result = ex.ToLogString();

        // Assert
        result.ShouldContain("Data:");
        result.ShouldContain("CustomKey1: Value1");
        result.ShouldContain("CustomKey2: 42");
    }
}