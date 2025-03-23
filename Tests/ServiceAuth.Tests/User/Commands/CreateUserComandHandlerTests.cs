using Application.DTOs.User;
using Application.Features.User.Requests.Commands;
using Application.Models.User;
using Cache.Contracts;
using EmailSender.Contracts;
using FluentValidation;
using HashProvider.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using ServiceAuth.Tests.Common;
using Tools;


namespace ServiceAuth.Tests.User.Commands
{
    public class CreateUserComandHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;
        public TestEmailSender EmailSender { get; set; } = null!;
        public RedisCustomMemoryCacheWithEvents RedisWithEvents { get; set; } = null!;
        public CreateUserSettings CreateUserSettings { get; set; } = null!;
        public IHashProvider HashProvider { get; set; } = null!;


        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            HashProvider = serviceProvider.GetRequiredService<IHashProvider>();
            Context = serviceProvider.GetRequiredService<AuthDbContext>();
            Context.Database.EnsureCreated();
            if (serviceProvider.GetRequiredService<IEmailSender>() is not TestEmailSender tes) throw new Exception();
            EmailSender = tes;
            if (serviceProvider.GetRequiredService<ICustomMemoryCache>() is not RedisCustomMemoryCacheWithEvents rwe) throw new Exception();
            RedisWithEvents = rwe;
            CreateUserSettings = serviceProvider.GetRequiredService<IOptions<CreateUserSettings>>().Value;
            EmailSender.Emails.Clear();
        }

        [Test]
        public void CreateUserComandHandler_CreateUserDtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateUserCommand { CreateUserDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateUserComandHandler_CreateUserDtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateUserCommand { CreateUserDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateUserComandHandler_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = new()
                {
                    Address = null,
                    Name = null,
                    Email = null
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateUserComandHandler_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = new()
                {
                    Email = string.Empty,
                    Name = string.Empty,
                    Address = string.Empty
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateUserComandHandler_EmailInvalid_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = new()
                {
                    Email = "SomeEmail223@",
                    Name = "Some name",
                    Address = "Some address"
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
            var func = async () => await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = new()
                {
                    Email = Users.First().Email,
                    Name = "Some name",
                    Address = "Some address"
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [TestCase("hflwahfawfk@wfaf")]
        [TestCase("asdfghj@awfa")]
        [TestCase("qwerty@awfa")]
        [TestCase("qwertywexrtcy@awfa")]
        [TestCase("qwertywexrtcywerdt@afw")]
        public async Task CreateUserComandHandler_CreateDtoValid_AddsUserToCacheForRegistrationNotToContextSendsEmailWithLoginParameters(string userEmail)
        {
            //arrange

            string cacheKey = string.Empty;
            TimeSpan timeSpan = TimeSpan.Zero;
            Domain.Models.User? user = null;

            RedisWithEvents.OnSet += (key, value, offset) =>
            {
                if (value is not Domain.Models.User uVal || uVal.Email != userEmail)
                {
                    return;
                }

                cacheKey = key;
                user = uVal;
                timeSpan = offset;
            };

            CreateUserDto createUserDto = new()
            {
                Email = userEmail,
                Name = "Some name",
                Address = "Some address"
            };

            //act

            await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = createUserDto
            });

            var sentEmail = EmailSender.LastSentEmail;

            var results = sentEmail.Body.ParseExact(CreateUserSettings.BodyMessageFormat);

            string emailFromMessage = results[0];
            string passwordFromMessage = results[1];

            if (user == null)
            {
                Assert.Fail($"User with email '{userEmail}' wasn't set in cache");
                return;
            }

            HashProvider.HashAlgorithmName = user.HashAlgorithm;
            HashProvider.Encoding = System.Text.Encoding.GetEncoding(user.StringEncoding);
            var hashFromEmailPassword = HashProvider.GetHash(passwordFromMessage);

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(user, Is.Not.Null);
                Assert.That(user.Email, Is.EqualTo(createUserDto.Email));
                Assert.That(user.Address, Is.EqualTo(createUserDto.Address));
                Assert.That(user.Name, Is.EqualTo(createUserDto.Name));
                Assert.That(Users.Any(u => u.Email == user.Email), Is.False);
                Assert.That(Users, Does.Not.Contain(user));
                Assert.That(cacheKey, Is.EqualTo(string.Format(CreateUserSettings.KeyForRegistrationCachingFormat, user.Email)));
                Assert.That(emailFromMessage, Is.EqualTo(userEmail));
                Assert.That(timeSpan, Is.EqualTo(TimeSpan.FromHours(CreateUserSettings.EmailConfirmationTimeoutHours)));
                Assert.That(emailFromMessage, Is.EqualTo(user.Email));
                Assert.That(emailFromMessage, Is.Not.Null.Or.Empty);
                Assert.That(passwordFromMessage, Is.Not.Null.Or.Empty);
                Assert.That(hashFromEmailPassword, Is.EqualTo(user.PasswordHash));
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