using ExceptionHelpers;
using LoggingHelpers.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StringHelpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LoggingHelpers;

public class InfoMessageBuilder
{
    public static string BuildLoggingInfoMessage<TState>(
        EventId eventId, 
        LogLevel logLevel, 
        TState state, 
        Exception? exception, 
        Func<TState, Exception?, string> formatter, 
        HttpContext? httpContext, 
        string appName, 
        int maxMsgLength)
    {
        var formatterMessage = formatter(state, exception);

        //Chck whether there's any info to report
        if (string.IsNullOrEmpty(formatterMessage))
            return "";

        StringBuilder messageBuilder = new();

        messageBuilder
           .AppendLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"))
           .AppendLine()
           .AppendLine($"Level: {logLevel.ToString()}")
           .AppendLine()
           .AppendLine($"Event: {eventId.Name} - {eventId.Id}")
           .AppendLine()
           .AppendLine($"Application: {appName ?? "???"}")
           .AppendLine()
           .AppendLine($"{GetHttpContextInfo(httpContext)}")
           .AppendLine()
           .AppendLine(formatterMessage)
           .AppendLine()
           .AppendLine(exception?.InnerException?.Message)
           .AppendLine()
           .AppendLine(exception?.InnerException?.StackTrace)
           .AppendLine();

        if (httpContext != null)
            messageBuilder.AppendLine(httpContext.GetUserInfoToString());


        messageBuilder.AppendLine(exception?.ToLogString());
        if (exception != null)
            messageBuilder.AppendLine(exception?.InnerException?.ToLogString());


        messageBuilder.AppendLine();
        messageBuilder.AppendLine("Stack Trace");
        messageBuilder.AppendLine(Environment.StackTrace);

        var message = messageBuilder.ToString();
        message = message.Length > maxMsgLength ? message[..maxMsgLength] : message;



        return message;

    }

    //---------------------------------------//

    private static async Task<string> GetHttpContextInfo(HttpContext? httpContext)
    {
        if (httpContext == null)
            return string.Empty;

        StringBuilder messageBuilder = new();

        var body = await Body(httpContext);

        var remoteIpAddress = httpContext.Connection?.RemoteIpAddress;

        var path = httpContext.Request?.Path.Value;
        var query = httpContext.Request?.QueryString.Value;

        var formValues = FormValues(httpContext);

        messageBuilder
           .AppendLine($"RemoteIpAddress: {remoteIpAddress}");


        if (!path.IsNullOrWhiteSpace())
            messageBuilder
                .AppendLine()
               .AppendLine($"Request Path: {path}");

        if (!query.IsNullOrWhiteSpace())
            messageBuilder
                .AppendLine()
               .AppendLine($"Request Query: {query}");

        if (!body.IsNullOrWhiteSpace())
            messageBuilder
                .AppendLine()
               .AppendLine($"Request Body: {body}");


        if (!formValues.IsNullOrWhiteSpace())
            messageBuilder
                .AppendLine()
               .AppendLine($"Form Values: {formValues}");


        return messageBuilder.ToString();

    }

    //---------------------------------------//

    private static string FormValues(HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (request == null || !request.HasFormContentType)
            return "No Form!";


        var form = httpContext.Request.Form;
        if (form == null)
            return "No Form!";

        var sb = new StringBuilder();
        sb.AppendLine();

        foreach (var item in form)
            sb.AppendLine(item.Key + ": " + item.Value);

        return sb.ToString();

    }

    //---------------------------------------//

    private static async Task<string> Body(HttpContext httpContext)
    {
        try
        {
            httpContext.Request.EnableBuffering();
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;

            return body.Length > 3000 ? body[..3000] : body;
        }
        catch (ObjectDisposedException)
        {
            return "---";
        }
    }

}//Cls