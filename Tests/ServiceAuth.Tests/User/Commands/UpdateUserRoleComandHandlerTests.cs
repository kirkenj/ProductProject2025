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
    public class UpdateUserRoleCommandHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;
        public IEnumerable<Domain.Models.Role> Roles => Context.Roles;


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
        public void UpdateUserRoleComandHandlerTests_DtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act;
            var func = async () => await Mediator.Send(new UpdateUserRoleCommand { UpdateUserRoleDTO = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserRoleComandHandlerTests_DtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserRoleCommand { UpdateUserRoleDTO = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateUserRoleComandHandlerTests_ArgumentsAreDefault_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateUserRoleCommand
            {
                UpdateUserRoleDTO = new UpdateUserRoleDTO { Id = default, RoleID = default }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task UpdateUserRoleComandHandlerTests_UserIdNotContained_ReturnsBadRequest()
        {
            //arrange


            //act

            var result = await Mediator.Send(new UpdateUserRoleCommand
            {
                UpdateUserRoleDTO = new() { Id = Guid.NewGuid(), RoleID = Roles.Last().Id }
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
        public void UpdateUserRoleComandHandlerTests_RoleIdNotContained_ThrowsValidationException()
        {
            //arrange


            //act

            var func = async () => await Mediator.Send(new UpdateUserRoleCommand
            {
                UpdateUserRoleDTO = new()
                {
                    Id = Users.First().Id,
                    RoleID = Random.Shared.Next()
                }
            });

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(func, Throws.TypeOf<ValidationException>());
            });
        }

        [Test]
        public async Task UpdateUserRoleComandHandlerTests_ArgumentsValid_UpdatesValue()
        {
            //arrange

            var user = Users.First();

            var role = Roles.Last();

            var mayBeModifiedFields = new string[] { nameof(user.RoleID) };

            var clonedUserBeforeUpdate = JsonCloner.Clone(user);

            var updateDto = new UpdateUserRoleDTO()
            {
                Id = user.Id,
                RoleID = role.Id
            };
            //act

            var result = await Mediator.Send(new UpdateUserRoleCommand
            {
                UpdateUserRoleDTO = updateDto
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

                Assert.That(user.RoleID, Is.EqualTo(updateDto.RoleID));
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