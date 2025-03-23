using Application.DTOs.User;
using Application.Features.User.Requests.Queries;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using ServiceAuth.Tests.Common;


namespace ServiceAuth.Tests.User.Queries
{
    public class GetUsersHandlerTests
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
        public async Task GetUserListRequest_ReturnsAllValues()
        {
            //arrange
            var expectedResult = Mapper.Map<List<UserDto>>(Users);

            //act
            var result = await Mediator.Send(new GetUserListRequest() { });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result, Is.EquivalentTo(expectedResult));
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