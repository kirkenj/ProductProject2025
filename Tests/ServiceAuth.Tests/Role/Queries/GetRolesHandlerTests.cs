using Application.DTOs.Role;
using Application.Features.Role.Requests.Queries;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ServiceAuth.Tests.Common;


namespace ServiceAuth.Tests.Role.Queries
{
    public class GetRolesHandlerTests
    {
        public IMapper Mapper { get; set; } = null!;
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; }
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
        public async Task GetRoleListRequest_ReturnsValues()
        {
            //arrange
            var expectedResult = Mapper.Map<List<RoleDto>>(Roles);

            //act
            var result = await Mediator.Send(new GetRoleListRequest());

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.EquivalentTo(expectedResult));
                Assert.That(result.Success, Is.True);
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