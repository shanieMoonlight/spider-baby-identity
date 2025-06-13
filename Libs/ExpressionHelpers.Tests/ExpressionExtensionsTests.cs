using Shouldly;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ExpressionHelpers.Tests;

public class ExpressionExtensionsTests
{
    #region Test Classes
    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public Address? HomeAddress { get; set; }
        public int? NullableAge { get; set; }
    }

    private class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
    }
    #endregion

    #region ToMemberExpression Tests
    [Fact]
    public void ToMemberExpression_SimpleProperty_ReturnsCorrectExpression()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        string propertyName = "Name";

        // Act
        var result = param.ToMemberExpression(propertyName);

        // Assert
        result.ShouldNotBeNull();
        result.Member.Name.ShouldBe("Name");
        result.Type.ShouldBe(typeof(string));
    }

    [Fact]
    public void ToMemberExpression_NestedProperty_ReturnsCorrectExpression()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        string propertyPath = "HomeAddress.Street";

        // Act
        var result = param.ToMemberExpression(propertyPath);

        // Assert
        result.ShouldNotBeNull();
        result.Member.Name.ShouldBe("Street");
        result.Type.ShouldBe(typeof(string));
    }

    [Fact]
    public void ToMemberExpression_CamelCasedProperty_WorksCorrectly()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        string propertyName = "name"; // camelCase instead of PascalCase

        // Act
        var result = param.ToMemberExpression(propertyName);

        // Assert
        result.ShouldNotBeNull();
        result.Member.Name.ShouldBe("Name");
        result.Type.ShouldBe(typeof(string));
    }

    [Fact]
    public void ToMemberExpression_InvalidProperty_ThrowsArgumentException()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        string invalidProperty = "NonExistentProperty";

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            param.ToMemberExpression(invalidProperty)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ToMemberExpression_EmptyOrNullPropertyName_ThrowsArgumentException(string? propertyName)
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            param.ToMemberExpression(propertyName!)
        );
    }
    #endregion

    #region CreateNullPropagationExpression Tests
    [Fact]
    public void CreateNullPropagationExpression_ReferenceType_CreatesCorrectExpression()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        var property = "Name";

        // Act
        var result = param.CreateNullPropagationExpression(property);

        // Assert
        result.ShouldBeAssignableTo<ConditionalExpression>();
        var conditional = (ConditionalExpression)result;
        conditional.Type.ShouldBe(typeof(string));
    }

    [Fact]
    public void CreateNullPropagationExpression_ValueType_WrapsInNullable()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        var property = "Age";

        // Act
        var result = param.CreateNullPropagationExpression(property);

        // Assert
        result.ShouldBeAssignableTo<ConditionalExpression>();
        var conditional = (ConditionalExpression)result;
        conditional.Type.ShouldBe(typeof(int?));
    }

    [Fact] 
    public void CreateNullPropagationExpression_NullableValueType_RemainsNullable()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        var property = "NullableAge";

        // Act
        var result = param.CreateNullPropagationExpression(property);

        // Assert
        result.ShouldBeAssignableTo<ConditionalExpression>();
        var conditional = (ConditionalExpression)result;
        conditional.Type.ShouldBe(typeof(int?));
    }

    [Fact]
    public void CreateNullPropagationExpression_InvalidProperty_ThrowsException()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");
        var invalidProperty = "NonExistentProperty";

        // Act & Assert
        Should.Throw<ArgumentException>(() => 
            param.CreateNullPropagationExpression(invalidProperty)
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreateNullPropagationExpression_EmptyOrNullProperty_ThrowsArgumentException(string? property)
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(Person), "p");

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            param.CreateNullPropagationExpression(property!)
        );
    }
    #endregion

    #region IsNullable Tests
    [Fact]
    public void IsNullable_NullableValueType_ReturnsTrue()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(int?), "n");

        // Act
        var result = param.IsNullable();

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsNullable_NonNullableValueType_ReturnsFalse()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(int), "i");

        // Act
        var result = param.IsNullable();

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsNullable_ReferenceType_ReturnsFalse()
    {
        // Arrange
        ParameterExpression param = Expression.Parameter(typeof(string), "s");

        // Act
        var result = param.IsNullable();

        // Assert
        result.ShouldBeFalse("Reference types aren't considered nullable by this method");
    }
    #endregion
}