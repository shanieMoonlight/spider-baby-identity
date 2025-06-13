using ID.GlobalSettings.Routes;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using System.Reflection;
using Xunit.Abstractions;

namespace ID.Presentation.Tests.Controllers;

/// <summary>
/// THis is to make sure that are controllers and actions match the Routes in Domain layer.
/// They are used for certain auth tasks.
/// They must stay in sync
/// </summary>
/// <param name="output"></param>
public class ControllerTests(ITestOutputHelper output)
{

    //------------------------------------//

    [Fact]
    public void ControllersExist()
    {
        // Arrange
        var routesType = typeof(IdRoutes);
        var nestedTypes = routesType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        var assembly = Assembly.GetAssembly(typeof(IdPresentationAssemblyReference)); // Replace with an actual controller type from your project

        foreach (var nestedType in nestedTypes)
        {
            var controllerName = nestedType.GetField("Controller", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)?.ToString() + "Controller";

            // Act
            var controllerType = assembly?.GetTypes().FirstOrDefault(t => t.Name.Equals(controllerName, StringComparison.OrdinalIgnoreCase));

            // Assert
            controllerType.ShouldNotBeNull();
        }
    }

    //------------------------------------//

    public static TheoryData<string, string> GetControllerAndActionNames()
    {
        var data = new TheoryData<string, string>();
        var routesType = typeof(IdRoutes);
        var nestedTypes = routesType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var nestedType in nestedTypes)
        {
            var controllerName = nestedType.GetField("Controller", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)?.ToString() + "Controller";
            var actionsType = nestedType.GetNestedType("Actions", BindingFlags.Public | BindingFlags.Static);

            if (actionsType == null)
                continue;

            var fields = actionsType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var actionName = field.GetValue(null)?.ToString();
                data.Add(controllerName, actionName!);
            }
        }

        return data;
    }


    //------------------------------------//

    [Theory]
    [MemberData(nameof(GetControllerAndActionNames))]
    public void ControllersHaveActions(string controllerName, string actionName)
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(IdPresentationAssemblyReference)); // Replace with an actual controller type from your project
        var controllerType = assembly?.GetTypes().FirstOrDefault(t => t.Name.Equals(controllerName, StringComparison.OrdinalIgnoreCase));

        // Act
        var methodInfo = controllerType?.GetMethods().FirstOrDefault(m => m.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));

        // Assert
        output.WriteLine($"Checking for action: {actionName} in controller: {controllerName}");
        methodInfo.ShouldNotBeNull();
    }


    //------------------------------------//
}
