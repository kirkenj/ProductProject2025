using Application.DTOs.User;
using Application.Features.User.Requests.Commands;
using Application.Models.User;
using AutoMapper;
using Cache.Contracts;
using EmailSender.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using ServiceAuth.Tests.Common;
using System.Security.Cryptography;
using System.Text;
using Tools;


namespace ServiceAuth.Tests.User.Commands
{
    public class LoginHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;
        public TestEmailSender EmailSender { get; set; } = null!;
        public RedisCustomMemoryCacheWithEvents RedisWithEvents { get; set; } = null!;
        public CreateUserSettings CreateUserSettings { get; set; } = null!;
        public IMapper Mapper { get; private set; }

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            Context = serviceProvider.GetRequiredService<AuthDbContext>();
            Context.Database.EnsureCreated();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
            if (serviceProvider.GetRequiredService<IEmailSender>() is not TestEmailSender tes) throw new Exception();
            EmailSender = tes;
            if (serviceProvider.GetRequiredService<ICustomMemoryCache>() is not RedisCustomMemoryCacheWithEvents rwe) throw new Exception();
            RedisWithEvents = rwe;
            CreateUserSettings = serviceProvider.GetRequiredService<IOptions<CreateUserSettings>>().Value;
            Context.ChangeTracker.Clear();
            EmailSender.Emails.Clear();
        }

        [Test]
        public void LoginHandler_LoginDtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new LoginRequest { LoginDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void LoginHandler_LoginDtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new LoginRequest { LoginDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void LoginHandler_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = null,
                    Password = null
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void LoginHandler_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = string.Empty,
                    Password = string.Empty
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void LoginHandler_EmailInvalid_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = "SomeEmail223@",
                    Password = "fwafaf"
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task LoginHandler_EmailTakenInvalidPassword_ReturnsBadRequest()
        {
            //arrange
            var email = Users.First().Email;

            //act
            var result = await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = email,
                    Password = Guid.NewGuid().ToString()
                }
            });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            });
        }

        [Test]
        public async Task LoginHandler_UserCreationInvalidPassword_ReturnsBadRequestUserWithTestingEmailNotAdded()
        {
            //arrange
            var email = Random.Shared.Next().ToString() + "hello@world";

            await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = new()
                {
                    Email = email,
                    Address = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString()
                }
            });

            //act

            var result = await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = email,
                    Password = Guid.NewGuid().ToString()
                }
            });
            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
                Assert.That(Users.Any(u => u.Email == email), Is.False);
            });
        }

        [Test]
        public async Task LoginHandler_UserCreationTimeIsUp_ReturnsBadRequestCacheGetReturnsNullContextDoesntContainValueWithTestingEmail()
        {
            //arrange
            var email = Random.Shared.Next().ToString() + "hello@world";

            await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = new()
                {
                    Email = email,
                    Address = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString()
                }
            });

            Domain.Models.User? gottenUser = null;
            string? cacheKey = null;

            RedisWithEvents.OnGet += (key, value) =>
            {
                cacheKey = key;
                if (value is Domain.Models.User uVal && uVal.Email == email)
                {
                    gottenUser = uVal;
                }
            };

            var lastEmail = EmailSender.LastSentEmail ?? throw new Exception();
            var password = lastEmail.Body.ParseExact(CreateUserSettings.BodyMessageFormat)[1];
            //act

            await Task.Delay(TimeSpan.FromHours(CreateUserSettings.EmailConfirmationTimeoutHours));
            var result = await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = email,
                    Password = password
                }
            });
            //assert
            Assert.Multiple(() =>
            {
                Assert.That(cacheKey, Is.EqualTo(string.Format(CreateUserSettings.KeyForRegistrationCachingFormat, email)));
                Assert.That(Users.Any(u => u.Email == email), Is.False);
                Assert.That(gottenUser, Is.Null);
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
            });
        }

        [TestCase("mfwafaw@fawf")]
        [TestCase("mfwafaw@fawfafw")]
        [TestCase("mfwafaw@fawffwaf")]
        [TestCase("mfwaawfawfaw@fawffwaf")]
        [TestCase("mfwaawfawawfawfaw@fawffwaf")]
        [TestCase("mfwaawfawawfawafwfaw@fawffwaf")]
        public async Task LoginHandler_UserCreation_ReturnsOkAddsValueToContextRemovesValueFromCache(string email)
        {
            //arrange

            var createUserDto = new CreateUserDto
            {
                Email = email,
                Address = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };

            await Mediator.Send(new CreateUserCommand
            {
                CreateUserDto = createUserDto
            });

            var lastEmail = EmailSender.LastSentEmail ?? throw new Exception();
            var password = lastEmail.Body.ParseExact(CreateUserSettings.BodyMessageFormat)[1];
            string? droppedKey = null;

            RedisWithEvents.OnRemove += (key) =>
            {
                droppedKey = key;
            };

            //act

            var result = await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = email,
                    Password = password
                }
            });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(droppedKey, Is.EqualTo(string.Format(CreateUserSettings.KeyForRegistrationCachingFormat, email)));
                Assert.That(Users.Any(u => u.Email == email), Is.True);
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Email, Is.EqualTo(createUserDto.Email));
                Assert.That(result.Result.Name, Is.EqualTo(createUserDto.Name));
                Assert.That(result.Result.Address, Is.EqualTo(createUserDto.Address));
                Assert.That(result.Result.Role, Is.Not.Null);
                Assert.That(result.Result.Role.Id, Is.EqualTo(CreateUserSettings.DefaultRoleID));
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            });
        }

        [Test]
        public async Task LoginHandler_RegularLogin_ReturnsOk()
        {
            //arrange
            string email = "test@test";
            string password = "SOMECOOLPWD228";


            string hashAlgorithmName = "MD5";
            HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashAlgorithmName) ?? throw new();
            Encoding encoding = Encoding.UTF8;
            var pwdBytes = encoding.GetBytes(password);
            var pwdHash = hashAlgorithm.ComputeHash(pwdBytes);
            var pwdHashString = encoding.GetString(pwdHash);
            Domain.Models.User u = new()
            {
                Id = Guid.NewGuid(),
                Login = "item.userData.login",
                RoleID = 2,
                Email = email,
                Name = "item.userData.name",
                Address = "Confidential",
                PasswordHash = pwdHashString,
                StringEncoding = encoding.BodyName,
                HashAlgorithm = hashAlgorithmName
            };

            Context.Add(u);
            await Context.SaveChangesAsync();
            var expectedResult = Mapper.Map<UserDto>(Users.First(u => u.Email == email));

            //act

            var result = await Mediator.Send(new LoginRequest
            {
                LoginDto = new()
                {
                    Email = email,
                    Password = password
                }
            });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.EqualTo(expectedResult));
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
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