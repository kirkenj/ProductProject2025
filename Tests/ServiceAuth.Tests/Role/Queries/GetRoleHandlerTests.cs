using Application.DTOs.Role;
using Application.Features.Role.Requests.Queries;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ServiceAuth.Tests.Common;
using System.Net;


namespace ServiceAuth.Tests.Role.Queries
{
    public class GetRoleHandlerTests
    {
        public IMapper Mapper { get; set; } = null!;
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.Role> Roles => Context.Roles;

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
        public async Task GetRoleListRequest_IdDefault_Returns404()
        {
            //arrange

            //act
            var result = await Mediator.Send(new GetRoleDtoRequest { Id = default });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetRoleListRequest_IdNotExcist_Returns404()
        {
            //arrange

            //act
            var result = await Mediator.Send(new GetRoleDtoRequest { Id = int.MaxValue });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetRoleListRequest_IdExcists_ReturnsValue()
        {
            //arrange
            var roles = Roles.ToArray();

            var role = roles[Random.Shared.Next(roles.Length)];

            var expectedResult = Mapper.Map<RoleDto>(role);

            //act
            var result = await Mediator.Send(new GetRoleDtoRequest { Id = role.Id });

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