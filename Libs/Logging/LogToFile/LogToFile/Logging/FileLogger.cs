using LoggingHelpers;
using LogToFile.Setup;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LogToFile.Logging;

public class FileLogger(
    LogToFileOptions settings,
    string categoryName,
    IHttpContextAccessor httpContextAccessor)
    : ILogger
{
    private readonly IHttpContextAccessor? _httpContextAccessor = httpContextAccessor;
    private static readonly string _nl = Environment.NewLine;
    private static readonly string _divider = $"{_nl}{_nl}################################################################################################################################################################{_nl}#~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~#{_nl}################################################################################################################################################################{_nl}{_nl}";

    // Semaphore to ensure thread-safe file operations
    private static readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    //-------------------------------//

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        ArgumentNullException.ThrowIfNull(formatter);
        var report = InfoMessageBuilder.BuildLoggingInfoMessage(
           eventId,
           logLevel,
           state,
           exception,
           formatter,
           _httpContextAccessor?.HttpContext,
           settings.AppName,
           settings.MaxMessageLength);

        // Fire and forget async operation for better performance with timeout
        _ = Task.Run(async () =>
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(settings.AsyncTimeoutSeconds));
                await WriteToFileAsync(report, cts.Token);
            }
            catch (OperationCanceledException)
            {
                var timeoutMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Log operation timed out after {settings.AsyncTimeoutSeconds} seconds";
                Console.WriteLine(timeoutMessage);
                Debug.WriteLine(timeoutMessage);
            }
        });
    }


    //-------------------------------//  


    private async Task WriteToFileAsync(string report, CancellationToken cancellationToken = default)
    {
        await _fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            await PrependToFileAsync(settings.FileFullPath, $"{_nl}{_divider}{_nl}", cancellationToken);
            await PrependToFileAsync(settings.FileFullPath, report, cancellationToken);
        }
        catch (Exception ex)
        {
            //Don't throw just log the error
            var errorMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Failed to write to log file: {ex.Message}";
            Console.WriteLine(errorMessage);
            Debug.WriteLine(errorMessage);
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    //-------------------------------//

    public static async Task PrependToFileAsync(string filePath, string text, CancellationToken cancellationToken = default)
    {
        try
        {
            // Read the existing content
            string existingContent = "";
            if (File.Exists(filePath))
                existingContent = await File.ReadAllTextAsync(filePath, cancellationToken);

            // Prepend the new text
            string newContent = text + Environment.NewLine + existingContent;

            // Write the new content back to the file
            await File.WriteAllTextAsync(filePath, newContent, cancellationToken);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error prepending text to {filePath}: {ex.Message}");
            throw; // Re-throw the exception to allow calling code to handle it
        }
    }

    //-------------------------------//

    /// <summary>
    /// Method to decide whether to log an event or not.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel) =>
        settings.Filter == null || settings.Filter(categoryName, logLevel);//IsEnabled

    //-------------------------------//

    IDisposable ILogger.BeginScope<TState>(TState state) => default!;


}//Cls


