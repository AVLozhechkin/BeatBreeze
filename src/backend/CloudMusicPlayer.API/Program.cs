using System.Text.Json.Serialization;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core;
using CloudMusicPlayer.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiLayer(builder.Configuration)
    .AddCoreLayer()
    .AddInfrastructureLayer("Data source = CloudMusicPlayer.db");

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        var enumConverter = new JsonStringEnumConverter();

        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.Converters.Add(enumConverter);
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CloudMusicPlayer API",
        Version = "v1",
        Description = "An API for CloudMusicPlayer",
    });
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler(_ => {});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseSpa(spa =>
{
    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
});


app.Run();
