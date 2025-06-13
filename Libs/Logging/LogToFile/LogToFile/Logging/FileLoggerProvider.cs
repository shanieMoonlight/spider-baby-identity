using LogToFile.Setup;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LogToFile.Logging;

public class FileLoggerProvider(IOptions<LogToFileOptions> _optionsProvider, IHttpContextAccessor httpContextAccessor) : ILoggerProvider
{
    private readonly LogToFileOptions _options = _optionsProvider.Value;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    //-------------------------------//

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new FileLogger(_options, name, httpContextAccessor));


    //-------------------------------//

    public void Dispose()
    {
        _loggers.Clear();

        GC.SuppressFinalize(this); // Ensures that the finalizer is suppressed for this object.
    }

}//Cls
