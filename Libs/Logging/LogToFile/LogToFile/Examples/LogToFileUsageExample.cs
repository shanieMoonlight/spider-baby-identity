using LogToFile.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogToFile.Examples;

/// <summary>
/// Example usage of the async LogToFile library
/// </summary>
public static class LogToFileUsageExample
{

    /// <summary>
    /// Basic setup example with default options
    /// </summary>
    public static void BasicSetup()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddLogToFile(options =>
            {
                options.AppName = "MyDebugApp";
                options.FileName = "debug_log";
                options.FileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                options.MinLevel = LogLevel.Information;
            });
        });

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("BasicExample");

        // Test the logger
        logger.LogInformation("This is an info message");
        logger.LogWarning("This is a warning message");
        logger.LogError("This is an error message");
    }


    //-------------------------//


    /// <summary>
    /// Advanced setup with custom async options
    /// </summary>
    public static void AdvancedSetup()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddLogToFile(options =>
            {
                options.AppName = "MyAdvancedApp";
                options.FileName = "advanced_debug_log";
                options.FileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Logs");
                options.MinLevel = LogLevel.Debug;
                options.MaxMessageLength = 50000;
                options.AsyncTimeoutSeconds = 60; // 1 minute timeout
            });
        });

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdvancedExample");

        // Test the logger with various scenarios
        logger.LogDebug("Debug information for troubleshooting");
        logger.LogInformation("Application started successfully");

        try
        {
            // Simulate an error
            throw new InvalidOperationException("Something went wrong!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during processing");
        }
    }


    //-------------------------//


    /// <summary>
    /// Stress test to demonstrate async performance
    /// </summary>
    public static async Task StressTest()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddLogToFile(options =>
            {
                options.AppName = "StressTest";
                options.FileName = "stress_test_log";
                options.MinLevel = LogLevel.Information;
                options.AsyncTimeoutSeconds = 120; // Longer timeout for stress test
            });
        });

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("StressTest");

        var tasks = new List<Task>();

        // Create multiple concurrent logging operations
        for (int i = 0; i < 100; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(() =>
            {
                logger.LogInformation($"Stress test message {taskId} from task {Task.CurrentId}");
                logger.LogWarning($"Warning message {taskId} with some additional data");
            }));
        }

        await Task.WhenAll(tasks);
        logger.LogInformation("Stress test completed successfully!");
    }
}
