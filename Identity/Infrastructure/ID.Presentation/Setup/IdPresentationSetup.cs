using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ID.Presentation.Setup;

/// <summary>
///  Setup Event pub/sub
/// </summary>
public static class IdPresentationSetup
{
    public static IServiceCollection AddMyIdPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(IdPresentationAssemblyReference).Assembly));
        return services;
    }

    //------------------------//


    public static IEndpointRouteBuilder MapMyIdControllers(this IEndpointRouteBuilder app)
    {
        app.MapControllers();
        return app;
    }

}//Cls
