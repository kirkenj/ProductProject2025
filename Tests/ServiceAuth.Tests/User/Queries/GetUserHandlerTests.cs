using Application.DTOs.User;
using Application.Features.User.Requests.Queries;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ServiceAuth.Tests.Common;
using System.Net;


namespace ServiceAuth.Tests.User.Queries
{
    public class GetUserHandlerTests
    {
        public IMapper Mapper { get; set; } = null!;
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            Context = serviceProvider.GetRequiredService<AuthDbContext>();
            Context.Database.EnsureCreated();
        }

        [Test]
        public async Task GetUserDtoRequest_IdDefault_Returns404()
        {
            //arrange

            //act
            var result = await Mediator.Send(new GetUserDtoRequest { Id = default });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetUserDtoRequest_IdNotExcist_Returns404()
        {
            //arrange

            //act
            var result = await Mediator.Send(new GetUserDtoRequest { Id = Guid.NewGuid() });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetUserDtoRequest_IdExcists_ReturnsValue()
        {
            //arrange
            var users = Users.ToArray();

            var user = users[Random.Shared.Next(users.Length)];

            var expectedResult = Mapper.Map<UserDto>(user);

            //act
            var result = await Mediator.Send(new GetUserDtoRequest { Id = user.Id });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.EqualTo(expectedResult));
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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