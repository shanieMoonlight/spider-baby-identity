namespace LogToFile.Setup;

#pragma warning disable IDE1006 // Naming Styles
internal class DefaultValues
{
    /// <summary>
    /// 100000
    /// </summary>
    internal const int MAX_MESSAGE_LENGTH = 100000;

    /// <summary>
    /// 30
    /// </summary>
    internal const int ASYNC_TIMEOUT_SECONDS = 30;

    /// <summary>
    /// Plain Text (.txt)
    /// </summary>
    internal const string FILE_EXTENSION = ".txt";


    /// <summary>
    /// "Logging Report"
    /// </summary>
    internal const string FILE_NAME = "Logging_Report.txt";

    /// <summary>
    /// Environment.SpecialFolder.Desktop
    /// </summary>
    internal static readonly string FILE_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

}//Cls
#pragma warning restore IDE1006 // Naming Styles

