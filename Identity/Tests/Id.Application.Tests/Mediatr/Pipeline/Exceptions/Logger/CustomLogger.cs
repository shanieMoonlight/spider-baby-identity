using Microsoft.Extensions.Logging;

namespace ID.Application.Tests.Mediatr.Pipeline.Exceptions.Logger;

public class CustomLogger<T> : ILogger<T>
{
    private readonly Dictionary<Exception, int> _exceptionLogCount = [];

    //------------------------------------//

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    => null;

    //------------------------------------//

    public bool IsEnabled(LogLevel logLevel) => true;

    //------------------------------------//

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine($"{logLevel}: {formatter(state, exception)}");
        if (exception != null)
            RecordException(exception);
    }

    //------------------------------------//

    public int GetExceptionCount<TException>() where TException : Exception =>
        _exceptionLogCount.Where(kvp => kvp.Key is TException).Sum(kvp => kvp.Value);


    //------------------------------------//

    private void RecordException(Exception exception)
    {
        if (_exceptionLogCount.TryGetValue(exception, out int value))
            _exceptionLogCount[exception] = ++value;
        else
            _exceptionLogCount[exception] = 1;
    }

}//Cls

