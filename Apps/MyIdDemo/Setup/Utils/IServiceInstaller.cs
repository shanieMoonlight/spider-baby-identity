using MyIdDemo.Setup.Data;

namespace MyIdDemo.Setup.Utils;

/// <summary>
/// Class for registering Services
/// </summary>
public interface IServiceInstaller
{
    /// <summary>
    /// Install some service dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public WebApplicationBuilder Install(WebApplicationBuilder builder, StartupData startupData);

}//Int