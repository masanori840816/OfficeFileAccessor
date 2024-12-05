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
    string serverUrl = builder.Configuration.GetValue<string>("ServerUrl") ?? "http://0.0.0.0:5170";
    builder.WebHost.UseUrls(serverUrl);

    string[] origins = builder.Configuration.GetSection("Origins").Get<string[]>() ?? [];
    const string AllowOrigins = "AllowOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AllowOrigins,
            builder => builder.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
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
            // stop reference loop.
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
    var app = builder.Build();
    
    if (builder.Environment.EnvironmentName != "Development")
    {
        app.UsePathBase("/officefiles/");
        app.UseStaticFiles(new StaticFileOptions {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "office-file-accessor/dist")),
            RequestPath = ""
        });
    }
    app.UseCors(AllowOrigins);
    app.UseRouting();
    //app.UseSession();
    app.MapStaticAssets();
    app.MapControllers();
    app.MapWhen(context => context.Request.Path.StartsWithSegments("/api") == false,
        b => {
            b.UseSpa(spa =>
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