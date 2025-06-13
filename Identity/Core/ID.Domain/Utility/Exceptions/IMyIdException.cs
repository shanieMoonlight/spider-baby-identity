namespace ID.Domain.Utility.Exceptions;

/// <summary>
/// A way to "tag" exception from this library so they can be easily caught in the GlobalErrorHandler
/// </summary>
/// <param name="msg"></param>
public class MyIdException(string msg, Exception? innerException = null)
    : Exception(msg, innerException)
{ }