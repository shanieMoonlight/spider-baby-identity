using Microsoft.Extensions.Logging;
using Moq;
using MyResults;

namespace ID.PhoneConfirmation.Tests.Utils;

internal class ExceptionUtils
{
    public static void VerifyExceptionLogging<TClass>(Mock<ILogger<TClass>> mockLogger, int eventId, Exception expectedException)
        where TClass : class
    {
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyBasicResultLogging<TClass>(Mock<ILogger<TClass>> mockLogger, int eventId, BasicResult result)
        where TClass : class
    {
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                result.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyLoggerWasCalled<TClass>(Mock<ILogger<TClass>> mockLogger, LogLevel logLevel, string expectedMessage)
        where TClass : class
    {
        mockLogger.Verify(
            l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    public static void VerifyLoggerWasCalled<TClass>(Mock<ILogger<TClass>> mockLogger, LogLevel logLevel, Exception expectedException)
        where TClass : class
    {
        mockLogger.Verify(
            l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
