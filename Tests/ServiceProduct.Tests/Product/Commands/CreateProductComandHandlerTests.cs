using Application.DTOs.Product;
using Application.Features.Product.Requests.Commands;
using FluentValidation;
using ServiceProduct.Tests.Common;



namespace ServiceProduct.Tests.User.Commands
{
    public class CreateProductComandHandlerTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            EmailSender.Emails.Clear();
        }


        [Test]
        public void CreateProductComandHandler_DtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateProductCommand { CreateProductDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateProductComandHandler_DtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateProductCommand { CreateProductDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateProductComandHandler_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateProductCommand
            {
                CreateProductDto = new()
                {
                    ProducerId = Guid.Empty,
                    Name = null,
                    Description = null,
                    CreationDate = default,
                    IsAvailable = false,
                    Price = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void CreateProductComandHandler_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new CreateProductCommand
            {
                CreateProductDto = new()
                {
                    ProducerId = Guid.Empty,
                    Name = string.Empty,
                    Description = string.Empty,
                    CreationDate = default,
                    IsAvailable = false,
                    Price = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task CreateProductComandHandler_ArgumentsValidUserEmailValid_AddsValueToContextSendsEmailThatContainsCreatedProductId()
        {
            //arrange
            var user = AuthClient._users.First(u => u.Email != null);

            var request = new CreateProductDto()
            {
                ProducerId = user.Id,
                Name = "Some product's name",
                Description = "Sidit zhadinaa s kilogramom konfet",
                CreationDate = DateTime.Now,
                IsAvailable = false,
                Price = 12
            };


            //act
            var result = await Mediator.Send(new CreateProductCommand
            {
                CreateProductDto = request
            });

            var lastEmail = EmailSender.LastSentEmail;

            //assert

            var createdProduct = Products.FirstOrDefault(p => p.Id == result.Result);

            Assert.Multiple(() =>
            {
                Assert.That(createdProduct, Is.Not.Null);
                Assert.That(createdProduct.Id, Is.EqualTo(result.Result));
                Assert.That(createdProduct.ProducerId, Is.EqualTo(request.ProducerId));
                Assert.That(createdProduct.IsAvailable, Is.EqualTo(request.IsAvailable));
                Assert.That(createdProduct.Name, Is.EqualTo(request.Name));
                Assert.That(createdProduct.Description, Is.EqualTo(request.Description));
                Assert.That(createdProduct.CreationDate, Is.EqualTo(request.CreationDate));
                Assert.That(lastEmail, Is.Not.Null);
                Assert.That(lastEmail.To, Is.EqualTo(user.Email));
                Assert.That(lastEmail.Body, Does.Contain(createdProduct.Id.ToString()));
            });
        }

        [Test]
        public async Task CreateProductComandHandler_ArgumentsValidUserEmailNull_AddsValueToContextNoEmail()
        {
            //arrange
            var user = AuthClient._users.First(u => u.Email == null);

            var request = new CreateProductDto()
            {
                ProducerId = user.Id,
                Name = "Some product's name",
                Description = "Sidit zhadinaa s kilogramom konfet",
                CreationDate = DateTime.Now,
                IsAvailable = false,
                Price = 12
            };


            //act
            var result = await Mediator.Send(new CreateProductCommand
            {
                CreateProductDto = request
            });

            var lastEmail = EmailSender.LastSentEmail;

            //assert

            var createdProduct = Products.FirstOrDefault(p => p.Id == result.Result);

            Assert.Multiple(() =>
            {
                Assert.That(createdProduct, Is.Not.Null);
                Assert.That(createdProduct.Id, Is.EqualTo(result.Result));
                Assert.That(createdProduct.ProducerId, Is.EqualTo(request.ProducerId));
                Assert.That(createdProduct.IsAvailable, Is.EqualTo(request.IsAvailable));
                Assert.That(createdProduct.Name, Is.EqualTo(request.Name));
                Assert.That(createdProduct.Description, Is.EqualTo(request.Description));
                Assert.That(createdProduct.CreationDate, Is.EqualTo(request.CreationDate));
                Assert.That(lastEmail, Is.Null);
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}