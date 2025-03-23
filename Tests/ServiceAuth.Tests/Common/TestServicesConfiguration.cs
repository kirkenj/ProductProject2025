using Application.Contracts.Infrastructure;
using Application.Contracts.Persistence;
using Application.DTOs.Role;
using Application.Features.Role.Handlers.Queries;
using Application.Features.User.Handlers.Commands;
using Application.MediatRBehaviors;
using Application.Models.User;
using Cache.Contracts;
using EmailSender.Contracts;
using FluentValidation;
using HashProvider.Contracts;
using Infrastructure.PasswordGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence;
using Persistence.Repositories;
using Repository.Models;
using System.Reflection;
using Tools;

namespace ServiceAuth.Tests.Common
{
    public static class TestServicesConfiguration
    {
        public static IServiceCollection ConfigureTestServices(this IServiceCollection services)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            services.AddAutoMapper(typeof(RoleDto).Assembly);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetRoleDetailHandler).Assembly);
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(CreateUserComandHandler).Assembly);

            services.Configure<ForgotPasswordSettings>(a =>
            {
                a.EmailBodyFormat = "sdfhjk{0}";
            });

            services.Configure<CreateUserSettings>(a =>
            {
                a.BodyMessageFormat = "{1}, {0}";
                a.EmailConfirmationTimeoutHours = 1d / 2400d;
                a.DefaultRoleID = 2;
                a.KeyForRegistrationCachingFormat = "Test user registration {0}";
            });

            services.Configure<UpdateUserEmailSettings>(a =>
            {
                a.UpdateUserEmailMessageBodyFormat = "sdfghjkl{0}";
                a.UpdateUserEmailCacheKeyFormat = "{0}fghjkl";
                a.EmailUpdateTimeOutHours = 1d / 2400d;
            });

            services.Configure<HashProvider.Models.HashProviderSettings>(a =>
            {
                a.EncodingName = System.Text.Encoding.UTF8.BodyName;
                a.HashAlgorithmName = "MD5";
            });

            services.AddLogging();
            services.AddSingleton<ICustomMemoryCache, RedisCustomMemoryCacheWithEvents>(sp => new("localhost:3300"));
            services.AddSingleton<IEmailSender, TestEmailSender>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHashProvider, HashProvider.Models.HashProvider>();
            services.AddScoped<IPasswordGenerator, PasswordGenerator>();
            services.AddScoped((a) => Mock.Of<ILogger<GenericCachingRepository<Domain.Models.Role, int>>>());

            return services;
        }
    }
}
