using System.Text.Json;
using CentralizedJwtAuthentication;
using Exceptions;
using ProductService.Api.ProductAPI.Middlewares;
using ProductService.Core.Application;
using ProductService.Infrastucture.Infrastucture;
using ProductService.Infrastucture.Persistence;

const string JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME = "JwtSettings";

Console.WriteLine("Application started");

var settingsJson = Environment.GetEnvironmentVariable(JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME) ?? throw new CouldNotGetEnvironmentVariableException(JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME);
JwtSettings jwtSettings = JsonSerializer.Deserialize<JwtSettings>(settingsJson) ?? throw new InvalidOperationException("Couldn't deserialize JwtSettings from environment");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureApplicationServices();
builder.Services.ConfigurePersistenceServices();
builder.Services.ConfigureJwtAuthentication(jwtSettings);
builder.Services.ConfigureInfrastructureServices(builder.Environment.IsDevelopment());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();