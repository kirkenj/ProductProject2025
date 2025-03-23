using Gateway.Controllers;
using Gateway.Extensions;
using Gateway.Models;
using Gateway.Services;
using Gateway.Services.Interfaces;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();



builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IOpenApiService, OpenApiService>();

builder.Services.AddScoped<OpenApiMiddleware>();

var openApiSettingsSection = builder.Configuration.GetSection(nameof(OpenApiServiceSettings));
builder.Services.Configure<OpenApiServiceSettings>(openApiSettingsSection);
var openApiSettings = openApiSettingsSection.Get<OpenApiServiceSettings>()
    ?? throw new Exception($"Couldn't get {nameof(OpenApiServiceSettings)}");


builder.Configuration.AddJsonFile("ocelot.json", false, true);

//builder.Configuration.ConfigureOcelot(openApiSettings);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    foreach (var service in openApiSettings.ServiceConfigs)
    {
        c.SwaggerEndpoint($"/{openApiSettings.OpenApiPathPrefixSegment}/{service.Name}", $"{service.Name} Api");
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<OpenApiMiddleware>();

await app.UseOcelot();

app.Run();
