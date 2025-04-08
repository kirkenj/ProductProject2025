using ConfigurationExtensions;
using Constants;
using Gateway.Extensions;
using Gateway.Middlewares;
using Gateway.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddScoped<OpenApiMiddleware>();

builder.Configuration.AddEnvironmentVariablesCustom(nameof(ServicesSettings));

var openApiSettingsSection = builder.Configuration.GetSection(nameof(ServicesSettings));
builder.Services.Configure<ServicesSettings>(openApiSettingsSection);
var openApiSettings = openApiSettingsSection.Get<ServicesSettings>()
    ?? throw new Exception($"Couldn't get {nameof(ServicesSettings)}");

builder.Configuration.ConfigureOcelot(openApiSettings);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy(ApiConstants.CORS_POLICY_NAME,
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});


var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{openApiSettings.OpenApiPathPrefixSegment}", "API V1");
});

app.UseCors(ApiConstants.CORS_POLICY_NAME);

app.UseMiddleware<OpenApiMiddleware>();

app.UseHttpsRedirection();

app.UseWebSockets();

await app.UseOcelot();

app.Run();