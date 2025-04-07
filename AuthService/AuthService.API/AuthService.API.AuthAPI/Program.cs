using AuthAPI.Middlewares;
using AuthService.API.AuthAPI.Registrations;
using AuthService.Core.Application;
using AuthService.Infrastructure.Infrastructure;
using AuthService.Infrastructure.Persistence;
using ConfigurationExtensions;
using Constants;
using Messaging.Kafka;

Console.WriteLine("Application started");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariablesCustom(nameof(KafkaSettings));
builder.Services.ConfigureApiServices(builder.Configuration);
builder.Services.ConfigurePersistenceServices();
builder.Services.ConfigureInfrastructureServices(builder.Configuration);
builder.Services.ConfigureApplicationServices(builder.Configuration);
builder.Services.ConfigureJwtAuthentication();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

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


builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ApiConstants.ADMIN_AUTH_POLICY_NAME, policy =>
    {
        policy.RequireRole(ApiConstants.ADMIN_AUTH_ROLE_NAME);
    });

var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseCors(ApiConstants.CORS_POLICY_NAME);

app.MapControllers();

app.Run();
