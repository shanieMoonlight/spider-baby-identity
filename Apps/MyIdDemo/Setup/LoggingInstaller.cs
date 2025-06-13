using LogToFile.Setup;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Utils;

namespace MyIdDemo.Setup;



//######################################//

/// <summary>
/// Install Logging stuff
/// </summary>
public class LoggingInstaller : IServiceInstaller
{

    /// <summary>
    /// Install some Logging dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public WebApplicationBuilder Install(WebApplicationBuilder builder, StartupData startupData)
    {

        var logging = builder.Logging; 
        var env = builder.Environment;

        logging.ClearProviders();


        if (env.IsDevelopment())
        {
            object value = logging.AddLogToFile(config =>
            {
                config.AppName = startupData.APP_NAME;
            });

        }

        return builder;

    }

}//Cls


//######################################//

/// <summary>
/// Install Logging stuff
/// </summary>
public static class LoggingInstallerExtensions
{

    /// <summary>
    /// Install some Logging dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public static WebApplicationBuilder InstallLogging(this WebApplicationBuilder builder, StartupData startupData) =>
        new LoggingInstaller().Install(builder, startupData);

}//Cls


//######################################//