using System.Runtime.Serialization;
using System.Text.Json;
using CentralizedJwtAuthentication;
using ConfigurationExtensions;
using Exceptions;
using Messaging.Kafka;
using Messaging.Kafka.Consumer;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.Consumers;
using NotificationService.Api.NotificationApi.Hubs;
using NotificationService.Api.NotificationApi.Services;
using NotificationService.Core.Application;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Infrastucture.Infrastucture;
using NotificationService.Infrastucture.Persistence;
using NotificationService.Infrastucture.Persistence.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddEnvironmentVariablesCustom(nameof(JwtSettings), nameof(MongoDbConfiguration), nameof(KafkaConsumerSettings), nameof(KafkaSettings));

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureJwtAuthentication(builder.Configuration);

builder.Services.AddHttpClient();
builder.Services.RegisterConsumers(builder.Configuration);
builder.Services.ConfigureApplicationServices();
builder.Services.RegisterInfrastructureService(builder.Configuration);
builder.Services.RegisterPersistenceServices(builder.Configuration);

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddTransient<ISignalRNotificationService, SignalRNotificationService<NotificationHub>>();

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