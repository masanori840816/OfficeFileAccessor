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
        configuration.RootPath = "office-file-accessor/dist";
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
    app.UseStaticFiles(new StaticFileOptions {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "office-file-accessor/dist")),
        RequestPath = ""
    });
    app.MapControllers();
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "office-file-accessor/dist";

        /*if (env.IsDevelopment())
        {
            spa.UseAngularCliServer(npmScript: "start");
        }*/
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