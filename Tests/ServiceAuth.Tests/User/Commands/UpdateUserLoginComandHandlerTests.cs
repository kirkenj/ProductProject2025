using Application.DTOs.User;
using Application.Features.User.Requests.Commands;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ServiceAuth.Tests.Common;
using Tools;


namespace ServiceAuth.Tests.User.Commands
{
    public class UpdateUserLoginComandHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;


        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            Context = serviceProvider.GetRequiredService<AuthDbContext>();
            Context.Database.EnsureCreated();
        }

        [Test]
        public void UpdateUserLoginComandHandlerTests_DtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act;
            var func = async () => await Mediator.Send(new UpdateUserLoginComand { UpdateUserLoginDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserLoginComandHandlerTests_DtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserLoginComand { UpdateUserLoginDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserLoginComandHandlerTests_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserLoginComand
            {
                UpdateUserLoginDto = new()
                {
                    Id = Guid.Empty,
                    NewLogin = null
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserLoginComandHandlerTests_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserLoginComand
            {
                UpdateUserLoginDto = new()
                {
                    Id = Guid.Empty,
                    NewLogin = string.Empty
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task UpdateUserLoginComandHandlerTests_IdNotContained_ReturnsBadRequest()
        {
            //arrange


            //act

            var result = await Mediator.Send(new UpdateUserLoginComand
            {
                UpdateUserLoginDto = new()
                {
                    Id = Guid.NewGuid(),
                    NewLogin = "Some name"
                }
            });

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task UpdateUserLoginComandHandlerTests_ArgumentsValid_UpdatesValue()
        {
            //arrange

            var user = Users.First();

            var mayBeModifiedFields = new string[] { nameof(user.Login) };

            var clonedUserBeforeUpdate = JsonCloner.Clone(user);

            var updateDto = new UpdateUserLoginDto()
            {
                Id = user.Id,
                NewLogin = "Some name"
            };
            //act

            var result = await Mediator.Send(new UpdateUserLoginComand
            {
                UpdateUserLoginDto = updateDto
            });

            //assert

            var cmpResult = FieldComparator.GetNotEqualPropertiesAndFieldsNames(clonedUserBeforeUpdate, user, nameof(user.Role));

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(user.Login, Is.EqualTo(updateDto.NewLogin));
                Assert.That(cmpResult, Is.SubsetOf(mayBeModifiedFields), $"Only these fields and properties may be modified: ({string.Join(", ", mayBeModifiedFields)})");
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