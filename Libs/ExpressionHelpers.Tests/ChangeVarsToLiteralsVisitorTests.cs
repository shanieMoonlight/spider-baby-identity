using Shouldly;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ExpressionHelpers.Tests;

public class ChangeVarsToLiteralsVisitorTests
{
    // Test classes
    private class TestClass
    {
        public int IntField = 42;
        public string StringField = "Hello";
        public NestedClass? NestedField = new NestedClass();
        public int IntProperty => 100;
        public string StringProperty { get; set; } = "World";
        public NestedClass? NestedProperty { get; set; } = new NestedClass();
        public int? NullableIntField = 123;
        public int? NullableIntPropertyWithValue => 456;
        public int? NullableIntPropertyWithoutValue => null;
    }

    private class NestedClass
    {
        public int NestedInt = 200;
        public string NestedString { get; set; } = "Nested";
    }

    [Fact]
    public void Visit_IntField_ReplacesWithConstant()
    {
        // Arrange
        var instance = new TestClass();
        Expression<Func<int>> expr = () => instance.IntField;
        var visitor = new ChangeVarsToLiteralsVisitor();

        // Act
        var result = visitor.Visit(expr);

        // Assert
        result.ShouldBeAssignableTo<Expression<Func<int>>>();
        var lambda = (Expression<Func<int>>)result;
        lambda.Body.ShouldBeOfType<ConstantExpression>();
        ((ConstantExpression)lambda.Body).Value.ShouldBe(42);
    }



    [Fact]
    public void Visit_StringProperty_ReplacesWithConstant()
    {
        // Arrange
        var instance = new TestClass();
        Expression<Func<string>> expr = () => instance.StringProperty;
        var visitor = new ChangeVarsToLiteralsVisitor();

        // Act
        var result = visitor.Visit(expr);

        // Assert
        result.ShouldBeAssignableTo<Expression<Func<string>>>();
        var lambda = (Expression<Func<string>>)result;
        lambda.Body.ShouldBeOfType<ConstantExpression>();
        ((ConstantExpression)lambda.Body).Value.ShouldBe("World");
    }



    [Fact]
    public void Visit_NestedMemberAccess_ReplacesWithConstant()
    {
        // Arrange
        var instance = new TestClass();
        Expression<Func<int>> expr = () => instance.NestedField!.NestedInt;
        var visitor = new ChangeVarsToLiteralsVisitor();

        // Act
        var result = visitor.Visit(expr);

        // Assert
        result.ShouldBeAssignableTo<Expression<Func<int>>>();
        var lambda = (Expression<Func<int>>)result;
        lambda.Body.ShouldBeOfType<ConstantExpression>();
        ((ConstantExpression)lambda.Body).Value.ShouldBe(200);
    }



    [Fact]
    public void Visit_NestedPropertyAccess_ReplacesWithConstant()
    {
        // Arrange
        var instance = new TestClass();
        Expression<Func<string>> expr = () => instance.NestedProperty!.NestedString;
        var visitor = new ChangeVarsToLiteralsVisitor();

        // Act
        var result = visitor.Visit(expr);

        // Assert
        result.ShouldBeAssignableTo<Expression<Func<string>>>();
        var lambda = (Expression<Func<string>>)result;
        lambda.Body.ShouldBeOfType<ConstantExpression>();
        ((ConstantExpression)lambda.Body).Value.ShouldBe("Nested");
    }



}