using CentralizedJwtAuthentication;
using ConfigurationExtensions;
using Constants;
using Messaging.Kafka;
using ProductService.Api.ProductAPI.Middlewares;
using ProductService.Core.Application;
using ProductService.Infrastucture.Infrastucture;
using ProductService.Infrastucture.Persistence;

Console.WriteLine("Application started");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariablesCustom(nameof(JwtSettings), nameof(KafkaSettings));

// Add services to the container.
builder.Services.ConfigureApplicationServices();
builder.Services.ConfigurePersistenceServices();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.ConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(o =>
{
    o.AddPolicy(ApiConstants.CORS_POLICY_NAME,
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseCors(ApiConstants.CORS_POLICY_NAME);

app.MapControllers();

app.Run();