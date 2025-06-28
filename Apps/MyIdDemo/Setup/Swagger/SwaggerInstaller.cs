using ID.Application.Customers;
using ID.OAuth.Google;
using ID.PhoneConfirmation;
using ID.Presentation;
using Microsoft.OpenApi.Models;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Utils;
using MyResults;
using System.Reflection;

namespace MyIdDemo.Setup.Swagger;



//######################################//

/// <summary>
/// Install Swagger stuff
/// </summary>
public class SwaggerInstaller : IServiceInstaller
{
    private static readonly Assembly?[] _assemblies =
    [
        Assembly.GetEntryAssembly(),
        IdPresentationAssemblyReference.Assembly,
        IdPhoneConfirmationAssemblyReference.Assembly,
        IdGoogleOAuthAssemblyReference.Assembly,
        IdApplicationCustomersAssemblyReference.Assembly
    ];

    //------------------------------------//

    public static BasicResult GetSwaggerDocsFilePath(Assembly assembly)
    {

        var xmlDocsFileName = assembly?.GetName().Name;

        if (string.IsNullOrWhiteSpace(xmlDocsFileName))
            return GenResult<string>.NotFoundResult("File not found");


        var xmlDocsFileNameWithExt = $"{xmlDocsFileName}.xml";
        var xmlDocsFilePath = Path.Combine(AppContext.BaseDirectory, xmlDocsFileNameWithExt);

        return !File.Exists(xmlDocsFilePath) 
            ? BasicResult.NotFoundResult(xmlDocsFilePath) 
            : BasicResult.Success(xmlDocsFilePath);

    }

    //------------------------------------//

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


            foreach (var assembly in _assemblies)
            {
                if (assembly != null)
                {
                    var xmlDocsFilePathResult = GetSwaggerDocsFilePath(assembly);
                    if (xmlDocsFilePathResult.Succeeded)
                        c.IncludeXmlComments(xmlDocsFilePathResult.Info);
                }
            }

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