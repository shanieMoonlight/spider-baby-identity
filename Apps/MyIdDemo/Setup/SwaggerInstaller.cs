using LogToFile.Setup;
using Microsoft.OpenApi.Models;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Utils;

namespace MyIdDemo.Setup;



//######################################//

/// <summary>
/// Install Swagger stuff
/// </summary>
public class SwaggerInstaller : IServiceInstaller
{

    /// <summary>
    /// Install some Swagger dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public WebApplicationBuilder Install(WebApplicationBuilder builder, StartupData startupData)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            // Define the security scheme for JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            });

            // Add security requirement to operations
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
     });
        });

        return builder;

    }

}//Cls


//######################################//

/// <summary>
/// Install Swagger stuff
/// </summary>
public static class SwaggerInstallerExtensions
{

    /// <summary>
    /// Install some Swagger dependencies
    /// </summary>
    /// <param name="builder">Application Builder</param>
    /// <param name="startupData">All the app config and settings</param>
    public static WebApplicationBuilder InstallSwagger(this WebApplicationBuilder builder, StartupData startupData) =>
        new SwaggerInstaller().Install(builder, startupData);

}//Cls

//######################################//