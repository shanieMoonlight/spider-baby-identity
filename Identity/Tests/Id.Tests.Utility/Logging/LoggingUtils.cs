using Microsoft.Extensions.Logging;
using Moq;
using MyResults;

namespace ID.Tests.Utility.Logging;
public static class LoggingUtils
{
    public static void VerifyExceptionLogging<TClass>(
        this Mock<ILogger<TClass>> _mockLogger,
        int eventId,
        Exception expectedException,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //- - - - - - - - - - - - //

    public static void VerifyExceptionLogging<TClass>(
        this Mock<ILogger<TClass>> _mockLogger,
        Exception expectedException,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //- - - - - - - - - - - - //


    public static void VerifyExceptionLogging<TClass, TException>(
        this Mock<ILogger<TClass>> _mockLogger,
        int eventId, Func<Times>? times = null)
        where TException : Exception
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<TException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //- - - - - - - - - - - - //

    public static void VerifyExceptionLogging<TClass>(
        this Mock<ILogger<TClass>> _mockLogger,
        int eventId,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //- - - - - - - - - - - - //

    public static void VerifyExceptionLogging<TClass>(
        this Mock<ILogger<TClass>> _mockLogger,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //------------------------//

    public static void VerifyErrorLogging<TClass>(
        this Mock<ILogger<TClass>> mockLogger,
        Func<Times>? times = null)
        where TClass : class => mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);

    //- - - - - - - - - - - - //

    public static void VerifyErrorLogging<TClass>(
        this Mock<ILogger<TClass>> mockLogger,
        EventId eventId,
        Func<Times>? times = null)
        where TClass : class => mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);

    //- - - - - - - - - - - - //

    public static void VerifyErrorLogging<TClass>(
        this Mock<ILogger<TClass>> mockLogger,
        EventId eventId,
        Func<object, bool> msgPredicate,
        Func<Times>? times = null)
        where TClass : class => mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.Is<EventId>(e => e == eventId),
                It.Is<It.IsAnyType>((v, t) => msgPredicate.Invoke(v)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);

    //- - - - - - - - - - - - //

    public static void VerifyErrorLogging<TClass>(
        this Mock<ILogger<TClass>> mockLogger,
        Func<object, bool> msgPredicate,
        Func<Times>? times = null)
        where TClass : class => mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => msgPredicate.Invoke(v)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);


    //------------------------//


    public static void VerifyBasicResultLogging<TClass>(
        this Mock<ILogger<TClass>> _mockLogger,
        BasicResult result,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                result.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //- - - - - - - - - - - - //


    public static void VerifyBasicResultLogging<TClass>(
        this Mock<ILogger<TClass>> _mockLogger,
        BasicResult result,
        int eventId,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                result.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    //- - - - - - - - - - - - //


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


    //------------------------//


    public static void VerifyGenResultLogging<TClass, T>(
        this Mock<ILogger<TClass>> _mockLogger,
        int eventId,
        GenResult<T> result,
        Func<Times>? times = null)
        where TClass : class
    {
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                eventId,
                It.Is<It.IsAnyType>((v, t) => true),
                result.Exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);
    }

    // Add helper to verify Warning level logs with message predicate
    public static void VerifyWarningLogging<TClass>(
        this Mock<ILogger<TClass>> mockLogger,
        Func<object, bool> msgPredicate,
        Func<Times>? times = null)
        where TClass : class => mockLogger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => msgPredicate.Invoke(v)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);

    // Optionally verify any warning was logged
    public static void VerifyWarningLogging<TClass>(
        this Mock<ILogger<TClass>> mockLogger,
        Func<Times>? times = null)
        where TClass : class => mockLogger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times ?? Times.Once);

}//Cls
