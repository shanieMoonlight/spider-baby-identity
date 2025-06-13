using ExceptionHelpers;
using Microsoft.Extensions.Logging;
using MyResults;
using System;

namespace LoggingHelpers;

public static class LoggingExtensions
{
    private const string _unknown_event = "UNKNOWN_EVENT";
    private static readonly string _nl = Environment.NewLine;

    //------------------------------------//

    public static void LogException<T>(this ILogger<T> logger, Exception e, int eventId, string eventName = _unknown_event) =>
        logger?.LogError(new EventId(eventId, eventName), e, "Exception Info: {expInfo}", $"{_nl}{e?.ToLogString()}{_nl}");

    //- - - - - - - - - - - - - - - - - - //

    public static void LogException<T>(this ILogger<T> logger, Exception e, string extraInfo, int eventId, string eventName = _unknown_event) =>
        logger?.LogError(new EventId(eventId, eventName), e, "Exception: {expInfo}Extra Info:{ extraInfo}", $"{_nl}{e?.ToLogString()}{_nl}", $"{_nl}{extraInfo}");
    
    //------------------------------------//

    public static void LogError<T>(this ILogger<T> logger, string info, int eventId, string eventName = _unknown_event) =>
        logger?.LogError(new EventId(eventId, eventName), "{info}", info);

    //------------------------------------//

    /// <summary>
    /// Log this GenResult
    /// </summary>
    /// <typeparam name="T">Type of Logger</typeparam>
    /// <typeparam name="GR">Type of GenResult</typeparam>
    /// <param name="logger">Logger</param>
    /// <param name="result">Failed GenResult</param>
    /// <param name="eventId">Id of error event</param>
    /// <param name="eventName">Name of Error event</param>
    public static void LogGenResultFailure<T, GR>(this ILogger<T> logger, GenResult<GR> result, int eventId, string eventName = _unknown_event)
    => logger?.LogError(new EventId(eventId, eventName), "Result: {result}", result);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Log this GenResult
    /// </summary>
    /// <typeparam name="T">Type of Logger</typeparam>
    /// <typeparam name="GR">Type of GenResult</typeparam>
    /// <param name="logger">Logger</param>
    /// <param name="result">Failed GenResult</param>
    /// <param name="eventId">Id of error event</param>
    /// <param name="eventName">Name of Error event</param>
    public static void LogBasicResultFailure<T>(this ILogger<T> logger, BasicResult result, int eventId, string eventName = _unknown_event)
    => logger?.LogError(new EventId(eventId, eventName), "Result: {result}", result);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Log this GenResult
    /// </summary>
    /// <typeparam name="T">Type of Logger</typeparam>
    /// <typeparam name="GR">Type of GenResult</typeparam>
    /// <param name="logger">Logger</param>
    /// <param name="result">Failed GenResult</param>
    /// <param name="extraInfo">Any extra info that you want to add. Default = null</param>
    /// <param name="eventId">Id of error event</param>
    /// <param name="eventName">NAme of Error event</param>
    public static void LogGenResult<T, GR>(this ILogger<T> logger, GenResult<GR> result, string extraInfo, int eventId, string eventName = _unknown_event)
    => logger?.LogError(new EventId(eventId, eventName), $"Result:{_nl}{result}{_nl}Extra Info:{_nl}{extraInfo}");

}//Cls

