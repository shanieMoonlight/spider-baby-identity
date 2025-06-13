using System.Text;

namespace ExceptionHelpers;

public static class ExceptionExtensions
{
    /// <summary>
    /// Gets all details from an exception in string form, formatted for a log report.
    /// </summary>
    /// <param name="ex">The exception to format</param>
    /// <returns>A formatted string containing exception details or empty string if exception is null</returns>

    public static string ToLogString(this Exception ex)
    {

        StringBuilder messageBuilder = new();

        // Special handling for AggregateException
        if (ex is AggregateException aggEx)
        {
            messageBuilder.AppendLine("Aggregate Exception with multiple inner exceptions:");
            int count = 0;
            foreach (var innerEx in aggEx.InnerExceptions)
            {
                messageBuilder
                    .AppendLine($"--- Inner Exception #{++count} ---")
                    .AppendException(innerEx)
                    .AppendLine("------------------------------");
            }
            return messageBuilder.ToString();
        }

        // Regular exception handling
        var currentEx = ex;
        do
        {
            messageBuilder
                .AppendException(currentEx)
                .AppendLine("------------------------------");

            currentEx = currentEx?.InnerException;
        }
        while (currentEx != null);

        return messageBuilder.ToString();
    }

    //----------------------//

    private static StringBuilder AppendException(this StringBuilder sb, Exception ex)
    {
        if (sb == null) sb = new StringBuilder();
        if (ex == null) return sb;

        sb
          .AppendLine()
          .AppendLine($"TimeStamp: {DateTime.UtcNow :dddd, dd MMMM yyyy HH:mm:ss.fff}")
          .AppendLine()
          .AppendLine($"Type:  {ex?.GetType()?.ToString() ?? "No type"}")
          .AppendLine()
          .AppendLine($"Message:  {ex?.Message ?? "No message"}")
          .AppendLine()
          .AppendLine($"Source:  {ex?.Source ?? "No Source"}")
          .AppendLine()
          .AppendLine($"HResult:  {ex?.HResult}")
          .AppendLine()
          .AppendLine($"Help Link:  {ex?.HelpLink ?? "No HelpLink"}")
          .AppendLine()
          .AppendLine($"Stack Trace:  {ex?.StackTrace ?? "No StackTrace"}")
          .AppendLine();


        if (ex?.Data.Count > 0)
        {
            sb
               .AppendLine($"Data:")
               .AppendLine();

            foreach (var key in ex.Data.Keys)
                sb.AppendLine($"{key}: {ex.Data[key]}");
        }


        return sb;

    }

}//Cls