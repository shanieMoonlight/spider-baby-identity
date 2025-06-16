using ID.API.Setup;
using ID.Demo.TestControllers.Setup;
using MyIdDemo.Middleware.Exceptions;
using MyIdDemo.Setup;
using MyIdDemo.Setup.Data;
using MyIdDemo.Setup.Events;
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
    options.AddDefaultPolicy(
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(_startupData.GetAllowedOrigins())
        )
    );



_services.AddControllers();

_services.AddEndpointsApiExplorer();

_builder
    .InstallSwagger(_startupData)
    .InstallHangfire(_startupData)
    .InstallMyId(_startupData)
    .InstallLogging(_startupData)
    ;

_services
    .AddMyIdDemoEvents()
    .AddMyIdDemoTestControllers();

//------------------------- Configure AppBuilder -------------------------//

var _app = _builder.Build();


_app.UseRouting();
_app.UseCors();


_app.UseCustomExceptionHandler(new MyIdDemoExceptionConverter());



// Configure the HTTP request pipeline.
if (_app.Environment.IsDevelopment())
{
    _app.UseSwagger();
    _app.UseSwaggerUI();
}

if (!_env.IsDevelopment())
{
    _app.UseHttpsRedirection();
}
else
{
    _app.UseWhen(ctx =>
           !ctx.Request.IsSwaggerRequest(),
            builder => builder.UseHttpsRedirection()
       );
}


_app.SetupStaticFilesUse(true);


//app.UseAuthorization();
_app.UseDemoHangfireJobs(_startupData);
_app.UseMyId();


_app.MapControllers();

_app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();

});

//_app.UseCookieDebugger();

_app.UseSpa(spa =>
{
    // To learn more about options for serving an Angular SPA from ASP.NET Core,
    // see https://go.microsoft.com/fwlink/?linkid=864501
    spa.Options.SourcePath = _startupData.SPA_STATIC_FILES_PATH;

    if (_env.IsDevelopment())
        spa.UseProxyToSpaDevelopmentServer($"http://localhost:{4208}");

});




if (_app.Environment.IsDevelopment())
    _app.UseDeveloperExceptionPage();


//JobStarter.StartTestJob1();


//-------------------------- Run the AppBuilder --------------------------//


_app.Run();
