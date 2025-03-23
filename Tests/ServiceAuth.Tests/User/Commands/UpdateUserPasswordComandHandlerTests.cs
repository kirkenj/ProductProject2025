using Application.DTOs.User;
using Application.Features.User.Requests.Commands;
using Application.Models.User;
using EmailSender.Contracts;
using FluentValidation;
using HashProvider.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using ServiceAuth.Tests.Common;
using System.Text;
using Tools;


namespace ServiceAuth.Tests.User.Commands
{
    public class UpdateUserPasswordComandHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;
        public TestEmailSender EmailSender { get; set; } = null!;
        public ForgotPasswordSettings forgotPasswordSettings { get; set; } = null!;
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
            forgotPasswordSettings = serviceProvider.GetRequiredService<IOptions<ForgotPasswordSettings>>().Value;
            EmailSender.Emails.Clear();
        }

        [Test]
        public void UpdateUserPasswordComandHandlerTests_DtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act;
            var func = async () => await Mediator.Send(new UpdateUserPasswordComand { UpdateUserPasswordDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserPasswordComandHandlerTests_DtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserPasswordComand { UpdateUserPasswordDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserPasswordComandHandlerTests_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserPasswordComand
            {
                UpdateUserPasswordDto = new()
                {
                    Id = default,
                    Password = null
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserPasswordComandHandlerTests_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserPasswordComand
            {
                UpdateUserPasswordDto = new()
                {
                    Id = default,
                    Password = string.Empty
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task ForgotPasswordComandHandler_IdNotContained_Returns494()
        {
            //arrange

            //act
            var func = await Mediator.Send(new UpdateUserPasswordComand
            {
                UpdateUserPasswordDto = new()
                {
                    Id = Guid.NewGuid(),
                    Password = "sdfghjkl"
                }
            });

            //assert
            Assert.That(func, Is.Not.Null);
            Assert.That(func.Success, Is.False);
            Assert.That(func.Message, Is.Not.Empty);
            Assert.That(func.Result, Is.Not.Empty);
            Assert.That(func.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        }

        [Test]
        public void ForgotPasswordComandHandler_PasswordInvalid_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserPasswordComand
            {
                UpdateUserPasswordDto = new()
                {
                    Id = Users.First().Id,
                    Password = "привет мир"
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }


        [TestCase("hflwahfawfkwfaf")]
        [TestCase("asdfghjawfa")]
        [TestCase("qwertyawfa")]
        [TestCase("qwertywexrtcyawfa")]
        [TestCase("qwertywexrtcywerdtafw")]
        public async Task ForgotPasswordComandHandler_EmailValid_SendsEmailWithPasswordUpdatesUserModel(string password)
        {
            //arrange

            var user = Users.First();

            var mayBeModifiedFields = new string[] { nameof(user.StringEncoding), nameof(user.HashAlgorithm), nameof(user.PasswordHash) };

            UpdateUserPasswordDto forgotPasswordDto = new()
            {
                Id = user.Id,
                Password = password,
            };

            var clonedUserBeforeUpdate = JsonCloner.Clone(user);

            //act

            var result = await Mediator.Send(new UpdateUserPasswordComand
            {
                UpdateUserPasswordDto = forgotPasswordDto
            });

            var newPassword = password;

            HashProvider.HashAlgorithmName = user.HashAlgorithm;
            HashProvider.Encoding = Encoding.GetEncoding(user.StringEncoding);
            var newPasswordHash = HashProvider.GetHash(newPassword);

            //assert


            var cmpResult = FieldComparator.GetNotEqualPropertiesAndFieldsNames(clonedUserBeforeUpdate, user, nameof(user.Role));

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(cmpResult, Is.SubsetOf(mayBeModifiedFields), $"Only these fields and properties may be modified: ({string.Join(", ", mayBeModifiedFields)})");

                Assert.That(newPasswordHash, Is.EqualTo(user.PasswordHash));
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