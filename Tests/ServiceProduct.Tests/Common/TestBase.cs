using AutoMapper;
using Clients.AuthApi;
using Domain.Models;
using EmailSender.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Tools;

namespace ServiceProduct.Tests.Common
{
    public class TestBase
    {
        public IMapper Mapper { get; set; } = null!;
        public IMediator Mediator { get; set; } = null!;
        public ProductDbContext Context { get; set; } = null!;
        public IEnumerable<Product> Products => Context.Products;
        public TestAuthClient AuthClient { get; set; } = null!;
        public TestEmailSender EmailSender { get; } = null!;


        public TestBase()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            Context = serviceProvider.GetRequiredService<ProductDbContext>();
            Context.Database.EnsureCreated();
            if (serviceProvider.GetRequiredService<IAuthApiClient>() is TestAuthClient tCleint)
            {
                AuthClient = tCleint;
            }

            if (serviceProvider.GetRequiredService<IEmailSender>() is TestEmailSender tSender)
            {
                EmailSender = tSender;
            }

            Context.Products.Add(new()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                Description = "wertyuop",
                IsAvailable = true,
                Name = "vbnm",
                Price = 12,
                ProducerId = AuthClient._users.First().Id,
            });

            Context.Products.Add(new()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                Description = "qwtqwtq",
                IsAvailable = false,
                Name = "sfaefaw",
                Price = 12,
                ProducerId = AuthClient._users.Last().Id
            });

            Context.Products.Add(new()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                Description = "qwtqwtq",
                IsAvailable = false,
                Name = "sfaefaw",
                Price = 12,
                ProducerId = AuthClient._users.First(u => u.Email == null).Id
            });

            Context.Products.Add(new()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                Description = "qwtqwtq",
                IsAvailable = false,
                Name = "sfaefaw",
                Price = 12,
                ProducerId = Guid.NewGuid()
            });

            Context.SaveChanges();

            Context.ChangeTracker.Clear();
        }
    }
}
