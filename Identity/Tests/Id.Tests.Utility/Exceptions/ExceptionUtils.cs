using Microsoft.Extensions.Logging;
using Moq;
using MyResults;

namespace Id.Tests.Utility.Exceptions;
public class ExceptionUtils
{
    public  static void VerifyExceptionLogging<TClass>(Mock<ILogger<TClass>> _mockLogger, int eventId, Exception expectedException)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyExceptionLogging<TClass, TException>(Mock<ILogger<TClass>> _mockLogger, int eventId) where TException : Exception
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<TException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }


    public static void VerifyExceptionLogging<TClass>(Mock<ILogger<TClass>> _mockLogger, int eventId)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyExceptionLogging<TClass>(Mock<ILogger<TClass>> _mockLogger)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyBasicResultLogging<TClass>(Mock<ILogger<TClass>> _mockLogger, int eventId, BasicResult result)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                result.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyGenResultLogging<TClass, T>(Mock<ILogger<TClass>> _mockLogger, int eventId, GenResult<T> result)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                result.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

}
