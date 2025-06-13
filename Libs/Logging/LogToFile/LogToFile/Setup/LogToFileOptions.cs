using Microsoft.Extensions.Logging;

namespace LogToFile.Setup;

public class LogToFileOptions
{
    /// <summary>
    /// Name of file to write to. Default =  <inheritdoc cref="DefaultValues.FILE_NAME"/>
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Type of file to write to. Default =  <inheritdoc cref="DefaultValues.FILE_EXTENSION"/>
    /// </summary>
    public string FileExtension { get; set; } = DefaultValues.FILE_EXTENSION;

    /// <summary>
    /// Type of file to write to. Default =  <inheritdoc cref="DefaultValues.FILE_DIRECTORY"/>
    /// </summary>
    public string FileDirectory { get; set; } = DefaultValues.FILE_DIRECTORY;

    /// <summary>
    /// Title of App. For identification.
    /// </summary>
    public string AppName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum length of Log Message. 
    /// Default =  <inheritdoc cref="DefaultValues.MAX_MESSAGE_LENGTH"/>
    /// </summary>
    public int MaxMessageLength { get; set; } = DefaultValues.MAX_MESSAGE_LENGTH;

    /// <summary>
    /// Timeout for async logging operations (in seconds). 
    /// Default =  <inheritdoc cref="DefaultValues.ASYNC_TIMEOUT_SECONDS"/>
    /// </summary>
    public int AsyncTimeoutSeconds { get; set; } = DefaultValues.ASYNC_TIMEOUT_SECONDS;

    /// <summary>
    /// Minimum level that the event must be in order to log it. <br/>
    /// Default = LogLevel.Error
    /// </summary>
    public LogLevel MinLevel { get; set; } = LogLevel.Error;

    //-------------------------//

    //Computed Properties

    /// <summary>
    ///Full path to Log File
    /// </summary>
    public string FileFullPath
    {
        get
        {
            var fullName = Path.ChangeExtension(FileName, FileExtension);
            return Path.Combine(FileDirectory, fullName);
        }
    }

    /// <summary>
    /// Minimum level that the event must be in order to log it. <br/>
    /// Default = LogLevel.Error
    /// </summary>
    public Func<string, LogLevel, bool>? Filter
    {
        get => ((_, logLevel) => logLevel >= MinLevel);
    }

}//Cls

