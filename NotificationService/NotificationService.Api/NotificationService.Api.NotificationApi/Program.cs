using System.Text.Json;
using CentralizedJwtAuthentication;
using Exceptions;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.Consumers;
using NotificationService.Api.NotificationApi.Hubs;
using NotificationService.Api.NotificationApi.Services;
using NotificationService.Core.Application;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Infrastucture.Infrastucture;
using NotificationService.Infrastucture.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settingsJson = Environment.GetEnvironmentVariable(nameof(JwtSettings)) ?? throw new CouldNotGetEnvironmentVariableException(nameof(JwtSettings));
JwtSettings jwtSettings = JsonSerializer.Deserialize<JwtSettings>(settingsJson) ?? throw new InvalidOperationException("Couldn't deserialize JwtSettings from environment");
builder.Services.ConfigureJwtAuthentication(jwtSettings);

builder.Services.AddHttpClient();
builder.Services.RegisterConsumers(builder.Configuration);
builder.Services.ConfigureApplicationServices();
builder.Services.RegisterInfrastructureService(builder.Configuration);
builder.Services.RegisterPersistenceServices(builder.Configuration);

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<ISignalRNotificationService, SignalRNotificationService<NotificationHub>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<NotificationHub>("/NotificationApiHub/hub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();