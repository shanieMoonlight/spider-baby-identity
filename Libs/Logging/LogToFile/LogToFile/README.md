# LogToFile - Async File Logging Library

An async file logging provider for .NET applications that writes log entries to text files with prepend functionality for easier debugging.

## Features

- ✅ **Async logging** - Non-blocking file operations using async/await
- ✅ **Thread-safe** - Uses SemaphoreSlim for concurrent write protection
- ✅ **Prepend logging** - New entries appear at the top of the file
- ✅ **Configurable timeouts** - Prevent hanging operations
- ✅ **Error handling** - Graceful handling of file system errors
- ✅ **Integration ready** - Works with Microsoft.Extensions.Logging

## Quick Start

### Basic Setup

```csharp
services.AddLogging(builder =>
{
    builder.AddLogToFile(options =>
    {
        options.AppName = "MyApp";
        options.FileName = "debug_log";
        options.FileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        options.MinLevel = LogLevel.Information;
    });
});
```

### Advanced Configuration

```csharp
services.AddLogging(builder =>
{
    builder.AddLogToFile(options =>
    {
        options.AppName = "MyAdvancedApp";
        options.FileName = "app_log";
        options.FileDirectory = @"C:\Logs";
        options.MinLevel = LogLevel.Debug;
        options.MaxMessageLength = 50000;
        options.AsyncTimeoutSeconds = 60; // 1 minute timeout
    });
});
```

## Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AppName` | string | - | **Required** - Application name for identification |
| `FileName` | string | `"Logging_Report.txt"` | Name of the log file |
| `FileExtension` | string | `".txt"` | File extension |
| `FileDirectory` | string | Desktop | Directory to store log files |
| `MinLevel` | LogLevel | `Error` | Minimum log level to write |
| `MaxMessageLength` | int | `100000` | Maximum length of log messages |
| `AsyncTimeoutSeconds` | int | `30` | Timeout for async operations |
| `IncludeTimestampInErrorOutput` | bool | `true` | Include timestamps in error console output |

## Usage Examples

### Basic Logging

```csharp
var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("MyClass");

logger.LogInformation("Application started");
logger.LogWarning("This is a warning");
logger.LogError(exception, "An error occurred");
```

### High-Performance Concurrent Logging

```csharp
// The logger handles concurrent writes automatically
var tasks = Enumerable.Range(0, 100).Select(i => 
    Task.Run(() => logger.LogInformation($"Message {i} from task {Task.CurrentId}"))
).ToArray();

await Task.WhenAll(tasks);
```

## How It Works

1. **Async Operation**: Log calls are wrapped in `Task.Run()` for non-blocking execution
2. **Thread Safety**: `SemaphoreSlim` ensures only one write operation at a time
3. **Prepend Logic**: Reads existing file content, prepends new content, writes back
4. **Error Handling**: Catches and logs exceptions without crashing the application
5. **Timeout Protection**: Cancellation tokens prevent hanging operations

## Performance Considerations

- **Prepend Operation**: Each log entry requires reading the entire file. Best for debugging with moderate log volumes
- **Async Benefits**: Non-blocking logging prevents UI/API delays
- **Thread Safety**: Minimal contention with SemaphoreSlim synchronization
- **Memory Usage**: Entire file content is loaded into memory during writes

## Best Practices

1. **Use for debugging**: This logger is optimized for development/debugging scenarios
2. **Monitor file size**: Large files will impact performance due to prepend operations
3. **Set appropriate timeouts**: Adjust `AsyncTimeoutSeconds` based on file size expectations
4. **Use proper log levels**: Configure `MinLevel` to avoid excessive logging
5. **Handle large messages**: Set `MaxMessageLength` to prevent memory issues

## Error Handling

The library gracefully handles common file system errors:

- **File access denied**: Logs error to console/debug output
- **Disk full**: Logs error without crashing application
- **Timeout**: Cancels operation and logs timeout message
- **Concurrent access**: Thread-safe operations prevent corruption

## Thread Safety

The library is fully thread-safe:

- Uses `SemaphoreSlim` for file access synchronization
- Static semaphore shared across all logger instances
- Async/await patterns prevent blocking

## Requirements

- .NET 8.0 or later
- Microsoft.Extensions.Logging
- Microsoft.AspNetCore.Http.Abstractions (for HTTP context)

## Dependencies

```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
<PackageReference Include="Microsoft.Extensions.Logging.Configuration" />
<PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" />
```
