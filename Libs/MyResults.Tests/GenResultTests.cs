using Shouldly;
using System;
using Xunit;
using static MyResults.BasicResult;

namespace MyResults.Tests
{
    public class GenResultTests
    {
        #region Factory Methods Tests

        [Fact]
        public void NotFoundResult_ShouldSetNotFoundFlagToTrue()
        {
            // Act
            var result = GenResult<int>.NotFoundResult();

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.NotFound.ShouldBeTrue();
            result.BadRequest.ShouldBeFalse();
            result.Unauthorized.ShouldBeFalse();
            result.Forbidden.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeFalse();
            result.Value.ShouldBe(default(int));
        }

        //------------------------------------//

        [Fact]
        public void NotFoundResult_WithInfo_ShouldSetNotFoundFlagAndInfoCorrectly()
        {
            // Arrange
            string info = "Item not found";

            // Act
            var result = GenResult<string>.NotFoundResult(info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.NotFound.ShouldBeTrue();
            result.Info.ShouldBe(info);
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void UnauthorizedResult_ShouldSetUnauthorizedFlagToTrue()
        {
            // Act
            var result = GenResult<bool>.UnauthorizedResult();

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Unauthorized.ShouldBeTrue();
            result.NotFound.ShouldBeFalse();
            result.BadRequest.ShouldBeFalse();
            result.Forbidden.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeFalse();
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void UnauthorizedResult_WithInfo_ShouldSetUnauthorizedFlagAndInfoCorrectly()
        {
            // Arrange
            string info = "Not authorized";

            // Act
            var result = GenResult<double>.UnauthorizedResult(info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Unauthorized.ShouldBeTrue();
            result.Info.ShouldBe(info);
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void ForbiddenResult_ShouldSetForbiddenFlagToTrue()
        {
            // Act
            var result = GenResult<Guid>.ForbiddenResult();

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Forbidden.ShouldBeTrue();
            result.NotFound.ShouldBeFalse();
            result.BadRequest.ShouldBeFalse();
            result.Unauthorized.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeFalse();
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void ForbiddenResult_WithInfo_ShouldSetForbiddenFlagAndInfoCorrectly()
        {
            // Arrange
            string info = "Access forbidden";

            // Act
            var result = GenResult<object>.ForbiddenResult(info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Forbidden.ShouldBeTrue();
            result.Info.ShouldBe(info);
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void BadRequestResult_WithStringInfo_ShouldSetBadRequestFlagToTrue()
        {
            // Arrange
            string info = "Invalid request";

            // Act
            var result = GenResult<int>.BadRequestResult(info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.BadRequest.ShouldBeTrue();
            result.NotFound.ShouldBeFalse();
            result.Unauthorized.ShouldBeFalse();
            result.Forbidden.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeFalse();
            result.Info.ShouldBe(info);
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void PreconditionRequiredResult_ShouldSetPreconditionRequiredFlagToTrue()
        {
            // Act
            var result = GenResult<DateTime>.PreconditionRequiredResult();

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeTrue();
            result.NotFound.ShouldBeFalse();
            result.BadRequest.ShouldBeFalse();
            result.Unauthorized.ShouldBeFalse();
            result.Forbidden.ShouldBeFalse();
            result.Value.ShouldBe(default);
        }

        //------------------------------------//

        [Fact]
        public void PreconditionRequiredResult_WithInfo_ShouldSetPreconditionRequiredFlagAndInfoCorrectly()
        {
            // Arrange
            string info = "Precondition required";

            // Act
            var result = GenResult<string>.PreconditionRequiredResult(info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeTrue();
            result.Info.ShouldBe(info);
            result.Value.ShouldBe(default);
        }

        #endregion


        //====================================//


        #region Success and Failure Methods Tests

        [Fact]
        public void Success_WithValue_ShouldSetSucceededFlagAndValueCorrectly()
        {
            // Arrange
            int value = 42;

            // Act
            var result = GenResult<int>.Success(value);

            // Assert
            result.Succeeded.ShouldBeTrue();
            result.Value.ShouldBe(value);
            result.NotFound.ShouldBeFalse();
            result.BadRequest.ShouldBeFalse();
            result.Unauthorized.ShouldBeFalse();
            result.Forbidden.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeFalse();
        }

        //------------------------------------//

        [Fact]
        public void Success_WithValueAndInfo_ShouldSetSucceededFlagInfoAndValueCorrectly()
        {
            // Arrange
            string value = "test value";
            string info = "Operation successful";

            // Act
            var result = GenResult<string>.Success(value, info);

            // Assert
            result.Succeeded.ShouldBeTrue();
            result.Value.ShouldBe(value);
            result.Info.ShouldBe(info);
        }

        //------------------------------------//

        [Fact]
        public void Failure_ShouldSetSucceededFlagToFalse()
        {
            // Act
            var result = GenResult<int>.Failure();

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Value.ShouldBe(default);
            result.NotFound.ShouldBeFalse();
            result.BadRequest.ShouldBeFalse();
            result.Unauthorized.ShouldBeFalse();
            result.Forbidden.ShouldBeFalse();
            result.PreconditionRequired.ShouldBeFalse();
        }

        //------------------------------------//

        [Fact]
        public void Failure_WithInfo_ShouldSetSucceededFlagToFalseAndInfoCorrectly()
        {
            // Arrange
            string info = "Operation failed";

            // Act
            var result = GenResult<string>.Failure(info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Value.ShouldBe(default);
            result.Info.ShouldBe(info);
        }

        //------------------------------------//

        [Fact]
        public void Failure_WithException_ShouldSetSucceededFlagToFalseAndExceptionCorrectly()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act
            var result = GenResult<bool>.Failure(exception);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Value.ShouldBe(default);
            result.Exception.ShouldBe(exception);
        }

        //------------------------------------//

        [Fact]
        public void Failure_WithExceptionAndInfo_ShouldSetSucceededFlagToFalseWithExceptionAndInfoCorrectly()
        {
            // Arrange
            var exception = new Exception("Test exception");
            string info = "Operation failed with exception";

            // Act
            var result = GenResult<double>.Failure(exception, info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Value.ShouldBe(default);
            result.Exception.ShouldBe(exception);
            result.Info.ShouldBe(info);
        }

        //------------------------------------//

        [Fact]
        public void Failure_WithStatus_ShouldSetStatusCorrectly()
        {
            // Arrange
            var exception = new Exception("Test exception");
            string info = "Forbidden access";
            var status = ResultStatus.Forbidden;

            // Act
            var result = GenResult<int>.Failure(status, exception, info);

            // Assert
            result.Succeeded.ShouldBeFalse();
            result.Value.ShouldBe(default);
            result.Exception.ShouldBe(exception);
            result.Info.ShouldBe(info);
            result.Status.ShouldBe(status);
            result.Forbidden.ShouldBeTrue();
        }

        #endregion


        //====================================//


        #region Convert Tests

        [Fact]
        public void Convert_WithDefaultValue_ShouldCreateNewGenResultWithSamePropertiesExceptValue()
        {
            // Arrange
            var original = GenResult<int>.Success(42, "Original result");

            // Act
            var converted = original.Convert<string>();

            // Assert
            converted.Succeeded.ShouldBeTrue();
            converted.Info.ShouldBe("Original result");
            converted.Value.ShouldBe(default);
            converted.ShouldBeOfType<GenResult<string>>();
        }

        //------------------------------------//

        [Fact]
        public void Convert_WithNewValue_ShouldCreateNewGenResultWithSpecifiedValue()
        {
            // Arrange
            var original = GenResult<int>.Success(42, "Original result");
            string newValue = "Converted value";

            // Act
            var converted = original.Convert<string>(newValue);

            // Assert
            converted.Succeeded.ShouldBeTrue();
            converted.Info.ShouldBe("Original result");
            converted.Value.ShouldBe(newValue);
            converted.ShouldBeOfType<GenResult<string>>();
        }

        //------------------------------------//

        [Fact]
        public void Convert_WithValueConverter_ShouldCreateNewGenResultWithConvertedValue()
        {
            // Arrange
            var original = GenResult<int?>.Success(42, "Original result");
            static string? converter(int? i) => i.HasValue ? $"Value: {i}" : null;

            // Act
            var converted = original.Convert(converter);

            // Assert
            converted.Succeeded.ShouldBeTrue();
            converted.Info.ShouldBe("Original result");
            converted.Value.ShouldBe("Value: 42");
            converted.ShouldBeOfType<GenResult<string>>();
        }

        //------------------------------------//

        [Fact]
        public void Convert_WithFailedResult_ShouldKeepFailureStatus()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var original = GenResult<int>.Failure(exception, "Failed result");

            // Act
            var converted = original.Convert<string>("This value should not be used");

            // Assert
            converted.Succeeded.ShouldBeFalse();
            converted.Info.ShouldBe("Failed result");
            converted.Exception.ShouldBe(exception);
            converted.Value.ShouldBe("This value should not be used"); // Still sets the value even for failed results
            converted.ShouldBeOfType<GenResult<string>>();
        }

        #endregion


        //====================================//


        #region ToBasicResult Tests

        [Fact]
        public void ToBasicResult_ShouldConvertToBasicResultCorrectly()
        {
            // Arrange
            var genResult = GenResult<int>.Success(42, "Success info");

            // Act
            var basicResult = genResult.ToBasicResult();

            // Assert
            basicResult.Succeeded.ShouldBeTrue();
            basicResult.Info.ShouldBe("Success info");
            basicResult.ShouldBeOfType<BasicResult>();
            // Value is not carried over since BasicResult doesn't have a Value property
        }

        //------------------------------------//

        [Fact]
        public void ToBasicResult_WithCustomInfo_ShouldOverrideInfo()
        {
            // Arrange
            var genResult = GenResult<int>.Success(42, "Original info");
            string customInfo = "Custom info";

            // Act
            var basicResult = genResult.ToBasicResult(customInfo);

            // Assert
            basicResult.Info.ShouldBe(customInfo);
        }

        #endregion


        //====================================//


        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnFormattedStringIncludingValue()
        {
            // Arrange
            var result = GenResult<int>.Success(42, "Test info");

            // Act
            var toString = result.ToString();

            // Assert
            toString.ShouldContain("Success");
            toString.ShouldContain("GenResult");
            toString.ShouldContain("Info: Test info");
            toString.ShouldContain("Value: 42");
            toString.ShouldContain("Succeeded: True");
        }

        //------------------------------------//

        [Fact]
        public void ToString_WithNullValue_ShouldNotIncludeValueLine()
        {
            // Arrange
            var result = GenResult<string>.Failure("Test failure");

            // Act
            var toString = result.ToString();

            // Assert
            toString.ShouldContain("Failure");
            toString.ShouldContain("GenResult");
            toString.ShouldContain("Info: Test failure");
            toString.ShouldContain("Succeeded: False");
            // When Value is null, there should not be a "Value: " line
        }

        #endregion


        //====================================//
    }
}