using System;
using Xunit;
using Shouldly;
using MyResults;

namespace MyResults.Tests;

public class BasicResultTests
{
    //====================================//


    #region Factory Methods Tests

    [Fact]
    public void NotFoundResult_ShouldSetNotFoundFlagToTrue()
    {
        // Act
        var result = BasicResult.NotFoundResult();

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.BadRequest.ShouldBeFalse();
        result.Unauthorized.ShouldBeFalse();
        result.Forbidden.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void NotFoundResult_WithInfo_ShouldSetNotFoundFlagAndInfoCorrectly()
    {
        // Arrange
        string info = "Item not found";

        // Act
        var result = BasicResult.NotFoundResult(info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(info);
    }

    //------------------------------------//

    [Fact]
    public void UnauthorizedResult_ShouldSetUnauthorizedFlagToTrue()
    {
        // Act
        var result = BasicResult.UnauthorizedResult();

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
        result.BadRequest.ShouldBeFalse();
        result.Forbidden.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void UnauthorizedResult_WithInfo_ShouldSetUnauthorizedFlagAndInfoCorrectly()
    {
        // Arrange
        string info = "Not authorized";

        // Act
        var result = BasicResult.UnauthorizedResult(info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(info);
    }

    //------------------------------------//

    [Fact]
    public void ForbiddenResult_ShouldSetForbiddenFlagToTrue()
    {
        // Act
        var result = BasicResult.ForbiddenResult();

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
        result.BadRequest.ShouldBeFalse();
        result.Unauthorized.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void ForbiddenResult_WithInfo_ShouldSetForbiddenFlagAndInfoCorrectly()
    {
        // Arrange
        string info = "Access forbidden";

        // Act
        var result = BasicResult.ForbiddenResult(info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
        result.Info.ShouldBe(info);
    }

    //------------------------------------//

    [Fact]
    public void BadRequestResult_WithStringInfo_ShouldSetBadRequestFlagToTrue()
    {
        // Arrange
        string info = "Invalid request";

        // Act
        var result = BasicResult.BadRequestResult(info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
        result.Unauthorized.ShouldBeFalse();
        result.Forbidden.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeFalse();
        result.Info.ShouldBe(info);
    }
    //------------------------------------//

    [Fact]
    public void BadRequestResult_WithObject_ShouldSetBadRequestFlagAndResponseCorrectly()
    {
        // Arrange
        var badRequestResponse = new { ErrorCode = 400, Message = "Bad request" };

        // Act
        var result = BasicResult.BadRequestResult(badRequestResponse);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.BadRequestResponse.ShouldBe(badRequestResponse);
    }
    //------------------------------------//

    [Fact]
    public void PreconditionRequiredResult_ShouldSetPreconditionRequiredFlagToTrue()
    {
        // Act
        var result = BasicResult.PreconditionRequiredResult();

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
        result.BadRequest.ShouldBeFalse();
        result.Unauthorized.ShouldBeFalse();
        result.Forbidden.ShouldBeFalse();
    }
    //------------------------------------//

    [Fact]
    public void PreconditionRequiredResult_WithInfo_ShouldSetPreconditionRequiredFlagAndInfoCorrectly()
    {
        // Arrange
        string info = "Precondition required";

        // Act
        var result = BasicResult.PreconditionRequiredResult(info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeTrue();
        result.Info.ShouldBe(info);
    }

    #endregion
    

    //====================================//


    #region Success and Failure Methods Tests

    
    [Fact]
    public void Success_ShouldSetSucceededFlagToTrue()
    {
        // Act
        var result = BasicResult.Success();

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
        result.BadRequest.ShouldBeFalse();
        result.Unauthorized.ShouldBeFalse();
        result.Forbidden.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeFalse();
    }
    //------------------------------------//

    [Fact]
    public void Success_WithInfo_ShouldSetSucceededFlagAndInfoCorrectly()
    {
        // Arrange
        string info = "Operation successful";

        // Act
        var result = BasicResult.Success(info);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(info);
    }
    //------------------------------------//

    [Fact]
    public void Failure_ShouldSetSucceededFlagToFalse()
    {
        // Act
        var result = BasicResult.Failure();

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeFalse();
        result.BadRequest.ShouldBeFalse();
        result.Unauthorized.ShouldBeFalse();
        result.Forbidden.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeFalse();
        result.Info.ShouldBe("Something went wrong ;[");
    }
    //------------------------------------//

    [Fact]
    public void Failure_WithInfo_ShouldSetSucceededFlagToFalseAndInfoCorrectly()
    {
        // Arrange
        string info = "Operation failed";

        // Act
        var result = BasicResult.Failure(info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(info);
    }
    //------------------------------------//

    [Fact]
    public void Failure_WithException_ShouldSetSucceededFlagToFalseAndExceptionCorrectly()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = BasicResult.Failure(exception);

        // Assert
        result.Succeeded.ShouldBeFalse();
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
        var result = BasicResult.Failure(exception, info);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Exception.ShouldBe(exception);
        result.Info.ShouldBe(info);
    }

    #endregion
    

    //====================================//


    #region Constructor Tests

    [Fact]
    public void Constructor_WithException_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var exception = new Exception("Test exception");
        string customMessage = "Custom error message";

        // Act
        var result = BasicResult.Failure(exception, customMessage);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Exception.ShouldBe(exception);
        result.Info.ShouldBe(customMessage);
    }

    #endregion
    

    //====================================//


    #region Conversion Tests

    
    [Fact]
    public void Convert_ShouldCreateGenResultWithSameProperties()
    {
        // Arrange
        var basicResult = BasicResult.NotFoundResult("Item not found");

        // Act
        var genResult = basicResult.Convert<int>();

        // Assert
        genResult.Succeeded.ShouldBeFalse();
        genResult.NotFound.ShouldBeTrue();
        genResult.Info.ShouldBe("Item not found");
        genResult.Value.ShouldBe(default);
    }

    #endregion


    //====================================//


    #region ToString Tests
            
    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var result = BasicResult.NotFoundResult("Item not found");

        // Act
        var toString = result.ToString();

        // Assert
        toString.ShouldContain("Status: NotFound");
        toString.ShouldContain("Info: Item not found");
        toString.ShouldNotContain("Succeeded:"); // Removed as Succeeded is no longer part of ToString
    }

    #endregion


    //====================================//
}
