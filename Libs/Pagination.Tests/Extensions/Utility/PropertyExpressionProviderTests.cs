using ExpressionHelpers;
using Pagination.Extensions.Utility;
using Shouldly;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Pagination.Tests.Extensions.Utility
{
    public class PropertyExpressionProviderTests
    {
        private class TestEntity
        {
            public string FirstName { get; set; } = string.Empty;
            public int Age { get; set; }
            public bool IsActive { get; set; }
            public DateTime? LastLoginDate { get; set; }
            public TestNestedEntity? NestedEntity { get; set; }
        }

        private class TestNestedEntity
        {
            public string Description { get; set; } = string.Empty;
        }

        private readonly ParameterExpression _param = Expression.Parameter(typeof(TestEntity), "x");

        [Theory]
        [InlineData("FirstName", "FirstName")]
        [InlineData("firstName", "FirstName")]
        [InlineData("age", "Age")]
        [InlineData("isActive", "IsActive")]
        public void GetPropertyExpression_ValidPropertyNames_ShouldCreateCorrectExpression(string fieldName, string expectedPropertyName)
        {
            // Act
            var result = PropertyExpressionProvider.GetPropertyExpression(_param, fieldName);

            // Assert
            result.ShouldNotBeNull();
            result.Member.Name.ShouldBe(expectedPropertyName);
            result.Expression.ShouldBe(_param);
        }

        [Fact]
        public void GetPropertyExpression_WithCustomPropertySelector_ShouldUseProvidedMapping()
        {
            // Arrange
            string fieldName = "name";
            Func<string, string> customSelector = (field) => field == "name" ? "FirstName" : field;

            // Act
            var result = PropertyExpressionProvider.GetPropertyExpression(_param, fieldName, customSelector);

            // Assert
            result.ShouldNotBeNull();
            result.Member.Name.ShouldBe("FirstName");
        }        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void GetPropertyExpression_EmptyFieldName_ShouldThrowArgumentException(string fieldName)
        {
            // Act & Assert
            var exception = Should.Throw<ArgumentException>(() => 
                PropertyExpressionProvider.GetPropertyExpression(_param, fieldName));
            
            exception.Message.ShouldContain("Field name cannot be empty");
            exception.ParamName.ShouldBe("field");
        }
        
        [Fact]
        public void GetPropertyExpression_NullFieldName_ShouldThrowArgumentException()
        {
            // Arrange
            string? fieldName = null;
            
            // Act & Assert
            var exception = Should.Throw<ArgumentException>(() => 
                PropertyExpressionProvider.GetPropertyExpression(_param, fieldName!));
            
            exception.Message.ShouldContain("Field name cannot be empty");
            exception.ParamName.ShouldBe("field");
        }

        [Fact]
        public void GetPropertyExpression_EmptyResolvedPropertyName_ShouldThrowArgumentException()
        {
            // Arrange
            string fieldName = "validField";
            Func<string, string> returnsEmptySelector = (field) => string.Empty;

            // Act & Assert
            var exception = Should.Throw<ArgumentException>(() => 
                PropertyExpressionProvider.GetPropertyExpression(_param, fieldName, returnsEmptySelector));
              exception.Message.ShouldContain("Property name resolved to empty");
            exception.ParamName.ShouldBe("field");
        }
    }
}
