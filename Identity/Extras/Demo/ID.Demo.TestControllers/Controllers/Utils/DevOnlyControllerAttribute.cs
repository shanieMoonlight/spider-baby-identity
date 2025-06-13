using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Hosting;

namespace ID.Demo.TestControllers.Controllers.Utils;

/// <summary>
/// Makes a controller only available in development environment.
/// In production, this acts like [NonController].
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DevOnlyControllerAttribute : Attribute, IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Development)
        {
            // Remove all actions to effectively disable the controller
            controller.Actions.Clear();

            // Hide from API Explorer
            controller.ApiExplorer.IsVisible = false;
        }
    }
}
