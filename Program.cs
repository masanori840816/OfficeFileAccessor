using System.Text.Json.Serialization;
using Microsoft.Extensions.FileProviders;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromFile("Nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    
    builder.Services.AddAntiforgery();
    /*builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });*/
    builder.Services.AddSpaStaticFiles(configuration =>
    {
        if (builder.Environment.EnvironmentName == "Development") 
        {
            configuration.RootPath = "office-file-accessor";
        }
        else
        {
            configuration.RootPath = "office-file-accessor/dist";
        }
    });
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // ReferenceLoopを切る.
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
    var app = builder.Build();
    
    app.UseRouting();
    //app.UseSession();
    app.MapStaticAssets();
    if (builder.Environment.EnvironmentName != "Development")
    {
        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "office-file-accessor/dist")),
            RequestPath = ""
        });
    }
    
    app.MapControllers();
    app.UseSpa(spa =>
    {
        if (builder.Environment.EnvironmentName == "Development") 
        {
            spa.Options.SourcePath = "office-file-accessor";
            spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
        }
        else
        {
            spa.Options.SourcePath = "office-file-accessor/dist";
        }
    });
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    logger.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    LogManager.Shutdown();
}