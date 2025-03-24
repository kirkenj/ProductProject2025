using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Application.MediatRBehaviors;
using Application.Models.User;
using AuthService.Core.Application.Contracts.Persistence;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ValidationException = FluentValidation.ValidationException;

namespace AuthService.Core.Application
{
    public static class ConfigureServiceRegistration
    {
        /// <summary>
        /// Has to be called after repositories configured because this registration calls repository to validate settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.Configure<CreateUserSettings>(configuration.GetSection("CreateUserSettings"));
            services.Configure<UpdateUserEmailSettings>(configuration.GetSection("UpdateUserEmailSettings"));
            services.Configure<ForgotPasswordSettings>(configuration.GetSection("ForgotPasswordSettings"));

            var provider = services.BuildServiceProvider();

            var listToValidate = new List<Type>()
            {
                typeof(IOptions<CreateUserSettings>),
                typeof(IOptions<UpdateUserEmailSettings>),
                typeof(IOptions<ForgotPasswordSettings>),
            };

            foreach (var item in listToValidate)
            {
                var getResult = provider.GetRequiredService(item);
                if (getResult is IOptions<IValidatableObject> validatable)
                {
                    var valiadtionResult = validatable.Value.Validate(new ValidationContext(validatable));
                    if (valiadtionResult.Any())
                        throw new ValidationException(validatable.Value.GetType().Name + "\n" +
                            string.Join("\n", valiadtionResult.Select(r => $"{string.Join(",", r.MemberNames)}: {r.ErrorMessage}")));
                }
            }


            var createUserSettings = provider.GetRequiredService<IOptions<CreateUserSettings>>().Value;
            var roleRep = provider.GetRequiredService<IRoleRepository>();
            var getRoleTask = roleRep.GetAsync(createUserSettings.DefaultRoleID);
            getRoleTask.Wait();
            if (getRoleTask.Result == null)
            {
                throw new ArgumentException($"Role with id '{createUserSettings.DefaultRoleID}' doesn't exist.");
            }

            return services;
        }
    }
}
