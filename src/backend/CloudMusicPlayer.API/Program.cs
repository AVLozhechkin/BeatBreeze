using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Utils;
using CloudMusicPlayer.Infrastructure.Utils;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLayer(builder.Configuration)
    .AddCoreLayer()
    .AddInfrastructureLayer(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();

app.UseSerilogRequestLogging();

app.UseExceptionHandler(_ => { });

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.UseSpa(spa =>
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    });
} else
{
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapFallbackToController("Index", "Fallback");
}

app.Run();
