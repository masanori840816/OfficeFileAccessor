using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using OfficeFileAccessor;
using OfficeFileAccessor.AppUsers;
using OfficeFileAccessor.AppUsers.Entities;
using OfficeFileAccessor.AppUsers.Repositories;
using OfficeFileAccessor.OfficeFiles;

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
    builder.Services.AddDbContext<OfficeFileAccessorContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("OfficeFileAccessor")));
    builder.Services.AddRazorPages();
    builder.Services.AddAntiforgery();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
            };
        });
    builder.Services.AddAuthorization();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });
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
    
    builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
        .AddUserStore<ApplicationUserStore>()
        .AddEntityFrameworkStores<OfficeFileAccessorContext>()
        .AddDefaultTokenProviders();
    builder.Services.AddScoped<IOfficeFileService, OfficeFileService>();
    builder.Services.AddScoped<IApplicationUsers, ApplicationUsers>();
    builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
    builder.Services.AddScoped<IUserTokens, UserTokens>();
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
    app.UseSession();
    app.Use(async (context, next) =>
    {
        if(context.Request.Cookies.TryGetValue("User-Token", out string? token))
        {
            if(string.IsNullOrEmpty(token) == false)
            {            
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }
        }        
        await next();
    });
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
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