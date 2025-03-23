using Application.DTOs.User;
using Application.Features.User.Requests.Commands;
using Application.Models.User;
using Cache.Contracts;
using EmailSender.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using ServiceAuth.Tests.Common;
using Tools;


namespace ServiceAuth.Tests.User.Commands
{
    public class SendTokenToUpdateUserEmailComandHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;
        public TestEmailSender EmailSender { get; set; } = null!;
        public RedisCustomMemoryCacheWithEvents RedisWithEvents { get; set; } = null!;
        public UpdateUserEmailSettings UpdateUserEmailSettings { get; set; } = null!;


        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            Context = serviceProvider.GetRequiredService<AuthDbContext>();
            Context.Database.EnsureCreated();
            if (serviceProvider.GetRequiredService<IEmailSender>() is not TestEmailSender tes) throw new Exception();
            EmailSender = tes;
            if (serviceProvider.GetRequiredService<ICustomMemoryCache>() is not RedisCustomMemoryCacheWithEvents rwe) throw new Exception();
            RedisWithEvents = rwe;
            UpdateUserEmailSettings = serviceProvider.GetRequiredService<IOptions<UpdateUserEmailSettings>>().Value;
            EmailSender.Emails.Clear();
        }

        [Test]
        public void SendTokenToUpdateUserEmailRequestHandler_UpdateUserEmailDtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new SendTokenToUpdateUserEmailRequest { UpdateUserEmailDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void SendTokenToUpdateUserEmailRequestHandler_UpdateUserEmailDtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new SendTokenToUpdateUserEmailRequest { UpdateUserEmailDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void SendTokenToUpdateUserEmailRequestHandler_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new SendTokenToUpdateUserEmailRequest
            {
                UpdateUserEmailDto = new()
                {
                    Email = null,
                    Id = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void SendTokenToUpdateUserEmailRequestHandler_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new SendTokenToUpdateUserEmailRequest
            {
                UpdateUserEmailDto = new()
                {
                    Email = string.Empty,
                    Id = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void SendTokenToUpdateUserEmailRequestHandler_EmailInvalid_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new SendTokenToUpdateUserEmailRequest
            {
                UpdateUserEmailDto = new()
                {
                    Email = "SomeEmail223@",
                    Id = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateUserComandHandler_EmailTaken_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new SendTokenToUpdateUserEmailRequest
            {
                UpdateUserEmailDto = new()
                {
                    Email = Users.First().Email,
                    Id = Users.Last().Id
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task CreateUserComandHandler_RequestValid_SendsEmailPendsRequestToCache()
        {
            var email = "Email@ma";

            //arrange
            var updateEmailDto = new UpdateUserEmailDto()
            {
                Email = email,
                Id = Users.First().Id
            };

            string? dtoCacheKey = null;
            TimeSpan? dtoTimeout = null;
            UpdateUserEmailDto dtoFromCache = null;

            RedisWithEvents.OnSet += (key, value, span) =>
            {
                if (value?.Equals(updateEmailDto) ?? false)
                {
                    dtoCacheKey = key;
                    dtoTimeout = span;
                    dtoFromCache = (UpdateUserEmailDto)value;
                }
            };

            //act
            await Mediator.Send(new SendTokenToUpdateUserEmailRequest
            {
                UpdateUserEmailDto = updateEmailDto
            });

            var message = EmailSender.LastSentEmail;

            var tokenFromMessage = message.Body.ParseExact(UpdateUserEmailSettings.UpdateUserEmailMessageBodyFormat)[0];

            var tokenFromCacheKey = dtoCacheKey?.ParseExact(UpdateUserEmailSettings.UpdateUserEmailCacheKeyFormat)[0] ?? null;

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(tokenFromCacheKey, Is.EqualTo(tokenFromMessage));
                Assert.That(updateEmailDto, Is.EqualTo(dtoFromCache));
                Assert.That(dtoTimeout, Is.EqualTo(TimeSpan.FromHours(UpdateUserEmailSettings.EmailUpdateTimeOutHours)));
            });
        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}