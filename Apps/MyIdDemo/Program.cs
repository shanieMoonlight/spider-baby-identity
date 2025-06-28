using ID.AddOns.Middleware.Swagger;
using ID.API.Setup;
using MyIdDemo.Middleware.Exceptions;
using MyIdDemo.Setup;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Events;
using MyIdDemo.Setup.Swagger;
using MyIdDemo.Setup.Utils;
using MyIdDemo.Utils;


//------------------------------ Variables ------------------------------//

var _builder = WebApplication.CreateBuilder(args);
var _env = _builder.Environment;
var _services = _builder.Services;
var _host = _builder.Host;
var _configuration = _builder.Configuration;
var _logging = _builder.Logging;
var _startupData = new StartupData(_configuration, _env);


//-------------------------- Configure Services --------------------------//

_services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(_startupData.GetAllowedOrigins())
        )
    );


//In production, the Angular files will be served from this directory
_services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = _startupData.SPA_STATIC_FILES_PATH;
});



_services.AddControllers();
_services.AddEndpointsApiExplorer();

_builder
    .InstallSwagger(_startupData)
    .InstallHangfire(_startupData)
    .InstallMyId(_startupData)
    .InstallLogging(_startupData)
    ;

_services
    .AddMyIdDemoEvents();

//------------------------- Configure AppBuilder -------------------------//

var _app = _builder.Build();


if (_app.Environment.IsDevelopment())
    _app.UseDeveloperExceptionPage();
else
    _app.UseCustomExceptionHandler(new MyIdDemoExceptionConverter());


_app.UseRouting();
_app.UseCors();


if (!_env.IsDevelopment())
    _app.UseHttpsRedirection();
else
    _app.UseWhen(ctx => !ctx.Request.IsSwaggerRequest(), builder => builder.UseHttpsRedirection());


if (!_env.IsDevelopment())
    _app.UseHsts();


//Authenticate everything from here on
_app.UseMyId();


#pragma warning disable ASP0014 // Suggest using top level route registrations
// Using UseEndpoints pattern despite ASP0014 warning because it provides better
// compatibility with SPA middleware and prevents route handling conflicts.
// Without it  spa.UseProxyToSpaDevelopmentServer catches everything and return NotFound page for api requests.
// In production environment, your SPA will most likely be served on a separate server. 
// This is a Demo app, so we're serving it here.
_app.UseEndpoints(endpoints =>
    endpoints.MapControllers());
#pragma warning restore ASP0014

//Swagger Auth comes before Swagger UI
if (!_app.Environment.IsDevelopment())
    _app.UseSwaggerAuthSuperTeam();

_app.UseSwagger()
    .UseSwaggerUI();



_app.UseDemoHangfireJobs(_startupData);


//_app.UseCookieDebugger();
_app.UseSpaStaticFilesWithUnknownTypes(true);

_app.UseSpa(spa =>
{
    spa.Options.SourcePath = _startupData.SPA_STATIC_FILES_PATH;

    if (_env.IsDevelopment())
        spa.UseProxyToSpaDevelopmentServer($"http://localhost:{4208}");

});


//-------------------------- Run the AppBuilder --------------------------//


_app.Run();
