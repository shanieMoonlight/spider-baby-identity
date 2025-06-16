using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Reflection;
using Xunit.Abstractions;

namespace ID.Application.Customers.Tests.Controllers;

/// <summary>
/// Tests to verify that HttpPatch actions use request bodies instead of query parameters
/// </summary>
public class HttpPatchBodyRequirementTests(ITestOutputHelper output)
{
    [Fact]
    public void HttpPatch_Actions_Should_Not_Have_Query_Parameters()
    {
        // Get the assembly containing all controllers
        var assembly = IdApplicationCustomersAssemblyReference.Assembly;

        // Find all controller types in the assembly
        var controllerTypes = assembly.GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                t.IsPublic &&
                (typeof(ControllerBase).IsAssignableFrom(t) || t.Name.EndsWith("Controller")))
            .ToList();

        output.WriteLine($"Found {controllerTypes.Count} controllers to analyze");

        var invalidActions = new List<string>();

        // Process each controller
        foreach (var controllerType in controllerTypes)
        {
            // Find all HttpPatch actions in this controller
            var patchActions = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.GetCustomAttributes<HttpPatchAttribute>().Any())
                .ToList();

            if (patchActions.Count > 0)
            {
                output.WriteLine($"Analyzing controller: {controllerType.Name} with {patchActions.Count} HttpPatch actions");
            }

            // Process each HttpPatch action
            foreach (var action in patchActions)
            {
                // Check if the method has parameters
                var parameters = action.GetParameters();

                // If no parameters, it's valid (no query params possible)
                if (parameters.Length == 0)
                    continue;

                bool isValid = true;
                string invalidParams = string.Empty;

                foreach (var param in parameters)
                {
                    // Check if parameter is from route (these are not allowed)
                    bool isFromRoute = param.GetCustomAttributes<FromRouteAttribute>().Any() ||
                                      action.GetCustomAttribute<HttpPatchAttribute>()?.Template?.Contains("{" + param.Name + "}") == true;

                    // Check if parameter is explicitly marked as from body or form
                    bool isFromBody = param.GetCustomAttributes<FromBodyAttribute>().Any();
                    bool isFromForm = param.GetCustomAttributes<FromFormAttribute>().Any();

                    // If parameter is neither from route nor from body/form, it's likely a query parameter
                    if (isFromRoute && !isFromBody && !isFromForm)
                    {
                        isValid = false;
                        invalidParams += $"{param.Name}, ";
                    }
                }

                if (!isValid)
                {
                    invalidActions.Add($"{controllerType.Name}.{action.Name} has invalid parameters: {invalidParams.TrimEnd(' ', ',')}");
                }
            }
        }

        // Assert that all HttpPatch actions are valid
        invalidActions.ShouldBeEmpty(
            $"The following HttpPatch actions have query parameters which violates API conventions:\n{string.Join("\n", invalidActions)}" +
            $"Patch actions must have use [FromBody] or [FromForm] for Swagger Client Model and Service Code-Generation"); 
    }

    //------------------------------------//

    //------------------------------------//

    [Fact]
    public void HttpPatch_Actions_In_All_Id_Assemblies_Should_Not_Have_Query_Parameters()
    {
        // Get all assemblies with namespace starting with "ID"
        var idAssemblies = GetAllIdAssemblies();

        output.WriteLine($"Found {idAssemblies.Count} assemblies with namespace starting with 'ID':");
        foreach (var assembly in idAssemblies)
        {
            output.WriteLine($"- {assembly.GetName().Name}");
        }

        var invalidActions = new List<string>();
        var totalControllerCount = 0;
        var totalPatchActionCount = 0;

        // Process each assembly
        foreach (var assembly in idAssemblies)
        {
            // Find all controller types in the assembly
            var controllerTypes = assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    t.IsPublic &&
                    (typeof(ControllerBase).IsAssignableFrom(t) || t.Name.EndsWith("Controller")))
                .ToList();

            totalControllerCount += controllerTypes.Count;

            // Process each controller
            foreach (var controllerType in controllerTypes)
            {
                // Find all HttpPatch actions in this controller
                var patchActions = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => method.GetCustomAttributes<HttpPatchAttribute>().Any())
                    .ToList();

                totalPatchActionCount += patchActions.Count;

                if (patchActions.Count > 0)
                {
                    output.WriteLine($"Analyzing controller: {controllerType.FullName} with {patchActions.Count} HttpPatch actions");
                }

                // Process each HttpPatch action
                foreach (var action in patchActions)
                {
                    // Check if the method has parameters
                    var parameters = action.GetParameters();

                    // If no parameters, it's valid (no query params possible)
                    if (parameters.Length == 0)
                        continue;

                    bool isValid = true;
                    string invalidParams = string.Empty;

                    foreach (var param in parameters)
                    {
                        // Check if parameter is from route (these are allowed)
                        bool isFromRoute = param.GetCustomAttributes<FromRouteAttribute>().Any() ||
                                          action.GetCustomAttribute<HttpPatchAttribute>()?.Template?.Contains("{" + param.Name + "}") == true;

                        // Check if parameter is explicitly marked as from body or form
                        bool isFromBody = param.GetCustomAttributes<FromBodyAttribute>().Any();
                        bool isFromForm = param.GetCustomAttributes<FromFormAttribute>().Any();

                        // If parameter is neither from route nor from body/form, it's likely a query parameter
                        if (!isFromRoute && !isFromBody && !isFromForm)
                        {
                            isValid = false;
                            invalidParams += $"{param.Name}, ";
                        }
                    }

                    if (!isValid)
                    {
                        invalidActions.Add($"{assembly.GetName().Name}: {controllerType.Name}.{action.Name} has invalid parameters: {invalidParams.TrimEnd(' ', ',')}");
                    }
                }
            }
        }

        output.WriteLine($"Analyzed {totalControllerCount} controllers with {totalPatchActionCount} HttpPatch actions across all ID assemblies");

        // Assert that all HttpPatch actions are valid
        invalidActions.ShouldBeEmpty(
            $"The following HttpPatch actions have query parameters which violates API conventions:\n{string.Join("\n", invalidActions)}\n" +
            $"Patch actions must have use [FromBody] or [FromForm] for Swagger Client Model and Service Code-Generation");
    }

    //------------------------------------//

    /// <summary>
    /// Gets all assemblies with namespaces starting with "ID"
    /// </summary>
    private List<Assembly> GetAllIdAssemblies()
    {
        // Get all currently loaded assemblies
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic) // Skip dynamic assemblies
            .ToList();

        // Filter for assemblies with namespaces starting with "ID"
        var idAssemblies = new List<Assembly>();

        foreach (var assembly in loadedAssemblies)
        {
            try
            {
                // Check if this assembly contains any types in a namespace starting with "ID."
                var hasIdNamespace = assembly.GetTypes()
                    .Any(t => t.Namespace != null && t.Namespace.StartsWith("ID."));

                if (hasIdNamespace)
                {
                    idAssemblies.Add(assembly);
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // Skip assemblies that can't be loaded (might be missing dependencies)
                output.WriteLine($"Could not load types from assembly: {assembly.GetName().Name}");
            }
            catch (Exception ex)
            {
                output.WriteLine($"Error processing assembly {assembly.GetName().Name}: {ex.Message}");
            }
        }

        return idAssemblies;
    }

    //------------------------------------//

}//Cls