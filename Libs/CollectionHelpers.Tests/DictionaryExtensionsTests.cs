using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace CollectionHelpers.Tests
{
    public class DictionaryExtensionsTests
    {
        #region AnyValues Tests
        
        [Fact]
        public void AnyValues_WithPopulatedDictionary_ReturnsTrue()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { ["key"] = 42 };
            
            // Act
            var result = dictionary.AnyValues();
            
            // Assert
            result.ShouldBeTrue();
        }
        
        [Fact]
        public void AnyValues_WithEmptyDictionary_ReturnsFalse()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();
            
            // Act
            var result = dictionary.AnyValues();
            
            // Assert
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void AnyValues_WithNullDictionary_ReturnsFalse()
        {
            // Arrange
            Dictionary<string, int>? dictionary = null;
            
            // Act
            var result = dictionary.AnyValues();
            
            // Assert
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void AnyValues_WithReadOnlyDictionary_Works()
        {
            // Arrange
            IReadOnlyDictionary<int, string> dictionary = new Dictionary<int, string> 
            {
                [1] = "one",
                [2] = "two"
            };
            
            // Act
            var result = dictionary.AnyValues();
            
            // Assert
            result.ShouldBeTrue();
        }
        
        #endregion


        
        #region ContainsKeySafe Tests
        
        [Fact]
        public void ContainsKeySafe_WithExistingKey_ReturnsTrue()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { ["test"] = 42 };
            
            // Act
            var result = dictionary.ContainsKeySafe("test");
            
            // Assert
            result.ShouldBeTrue();
        }
        
        [Fact]
        public void ContainsKeySafe_WithNonExistingKey_ReturnsFalse()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { ["test"] = 42 };
            
            // Act
            var result = dictionary.ContainsKeySafe("other");
            
            // Assert
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void ContainsKeySafe_WithNullKey_ReturnsFalse()
        {
            // Arrange
            var dictionary = new Dictionary<string, int> { ["test"] = 42 };
            
            // Act
            var result = dictionary.ContainsKeySafe(null);
            
            // Assert
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void ContainsKeySafe_WithNullDictionary_ReturnsFalse()
        {
            // Arrange
            Dictionary<string, int>? dictionary = null;
            
            // Act
            var result = dictionary.ContainsKeySafe("test");
            
            // Assert
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void ContainsKeySafe_WithCustomDictionaryImplementation_Works()
        {
            // Arrange
            IDictionary<string, bool> dictionary = new SortedDictionary<string, bool> 
            {
                ["yes"] = true,
                ["no"] = false
            };
            
            // Act
            var result = dictionary.ContainsKeySafe("yes");
            
            // Assert
            result.ShouldBeTrue();
        }
        
        #endregion


        
        #region Combined Scenarios
        
        [Fact]
        public void Extensions_WorkWithGenericInterfaces()
        {
            // Arrange
            var source = new Dictionary<int, string>() { [1] = "one" };
            IDictionary<int, string> dict = new Dictionary<int, string>(source);
            IReadOnlyDictionary<int, string> readOnly = new Dictionary<int, string>(source);
            
            // Act & Assert
            dict.AnyValues().ShouldBeTrue();
            readOnly.AnyValues().ShouldBeTrue();
            dict.ContainsKeySafe(1).ShouldBeTrue();
            dict.ContainsKeySafe(999).ShouldBeFalse();
        }
        
        #endregion
    }
}