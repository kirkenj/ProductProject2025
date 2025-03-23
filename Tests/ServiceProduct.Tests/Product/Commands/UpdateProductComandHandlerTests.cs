using Application.DTOs.Product;
using Application.Features.Product.Requests.Commands;
using Application.Features.Product.Requests.Queries;
using FluentValidation;
using ServiceProduct.Tests.Common;


namespace ServiceProduct.Tests.Product.Commands
{
    public class UpdateProductComandHandlerTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
            Context.ChangeTracker.Clear();
            EmailSender.Emails.Clear();
        }

        [Test]
        public void UpdateProductComandHandler_DtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act;
            var func = async () => await Mediator.Send(new UpdateProductCommand { UpdateProductDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateProductComandHandler_DtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateProductCommand { UpdateProductDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateProductComandHandler_ArgumentsAreDefault_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = new()
                {
                    Id = default,
                    ProducerId = default,
                    CreationDate = default,
                    Description = default,
                    IsAvailable = default,
                    Name = default,
                    Price = default,
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void UpdateProductComandHandler_IdNotContained_ThrowsValidationException()
        {
            //arrange


            //act

            var func = async () => await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = new()
                {
                    Id = Guid.NewGuid(),
                    ProducerId = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    Description = "default",
                    IsAvailable = false,
                    Name = "default",
                    Price = 2,
                }
            });

            //assert

            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task UpdateProductComandHandler_ArgumentsValidProducerIdNotUpdated_UpdatesValueNotSendsEmail()
        {
            //arrange

            var result1 = await Mediator.Send(new GetProductDtoRequest
            {
                Id = Products.First().Id,
            });

            ProductDto product = result1.Result ?? throw new Exception();

            var updateDto = new UpdateProductDto()
            {
                Id = product.Id,
                Name = Guid.NewGuid().ToString(),
                Price = (decimal)Random.Shared.NextDouble(),
                CreationDate = DateTime.Now.AddMinutes(12),
                Description = Guid.NewGuid().ToString(),
                IsAvailable = !product.IsAvailable,
                ProducerId = product.ProducerId,
            };
            //act

            Context.ChangeTracker.Clear();

            var result = await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = updateDto
            });

            //assert

            var contextValue = Mapper.Map<ProductDto>(Context.Products.First(x => x.Id == product.Id));

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(contextValue, Is.Not.EqualTo(product));
                Assert.That(contextValue.Id, Is.EqualTo(updateDto.Id));
                Assert.That(contextValue.Name, Is.EqualTo(updateDto.Name));
                Assert.That(contextValue.Price, Is.EqualTo(updateDto.Price));
                Assert.That(contextValue.CreationDate, Is.EqualTo(updateDto.CreationDate));
                Assert.That(contextValue.IsAvailable, Is.EqualTo(updateDto.IsAvailable));
                Assert.That(contextValue.ProducerId, Is.EqualTo(updateDto.ProducerId));

                Assert.That(EmailSender.LastSentEmail, Is.Null);
            });
        }

        [Test]
        public async Task UpdateProductComandHandler_ArgumentsValidBothEmailsAreNull_UpdatesValueDoesntSendsEmails()
        {
            //arrange
            var newOwner = AuthClient._users.First(u => u.Email == null);
            var prevOwner = AuthClient._users.First(u => u.Email == null && Products.Select(p => p.ProducerId).Contains(u.Id));

            var result1 = await Mediator.Send(new GetProductDtoRequest
            {
                Id = Products.First(p => p.ProducerId == prevOwner.Id).Id,
            });

            ProductDto product = result1.Result ?? throw new Exception();


            Context.ChangeTracker.Clear();


            var updateDto = new UpdateProductDto()
            {
                Id = product.Id,
                Name = Guid.NewGuid().ToString(),
                Price = (decimal)Random.Shared.NextDouble(),
                CreationDate = DateTime.Now.AddMinutes(12),
                Description = Guid.NewGuid().ToString(),
                IsAvailable = !product.IsAvailable,
                ProducerId = newOwner.Id,
            };
            //act

            Context.ChangeTracker.Clear();

            var result = await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = updateDto
            });

            //assert

            var contextValue = Mapper.Map<ProductDto>(Context.Products.First(x => x.Id == product.Id));

            var prevOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == prevOwner.Email);
            var newOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == newOwner.Email);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(contextValue, Is.Not.EqualTo(product));
                Assert.That(contextValue.Id, Is.EqualTo(updateDto.Id));
                Assert.That(contextValue.Name, Is.EqualTo(updateDto.Name));
                Assert.That(contextValue.Price, Is.EqualTo(updateDto.Price));
                Assert.That(contextValue.CreationDate, Is.EqualTo(updateDto.CreationDate));
                Assert.That(contextValue.IsAvailable, Is.EqualTo(updateDto.IsAvailable));
                Assert.That(contextValue.ProducerId, Is.EqualTo(updateDto.ProducerId));

                Assert.That(newOwnerEmail, Is.Null);
                Assert.That(prevOwnerEmail, Is.Null);
            });
        }

        [Test]
        public async Task UpdateProductComandHandler_ArgumentsValidPrevOwnerEmailNullNewOwnerNot_UpdatesValueDoesntSendsEmailToTheNew()
        {
            //arrange
            var newOwner = AuthClient._users.First(u => u.Email != null);
            var prevOwner = AuthClient._users.First(u => u.Email == null && Products.Select(p => p.ProducerId).Contains(u.Id));

            var result1 = await Mediator.Send(new GetProductDtoRequest
            {
                Id = Products.First(p => p.ProducerId == prevOwner.Id).Id,
            });

            ProductDto product = result1.Result ?? throw new Exception();


            Context.ChangeTracker.Clear();


            var updateDto = new UpdateProductDto()
            {
                Id = product.Id,
                Name = Guid.NewGuid().ToString(),
                Price = (decimal)Random.Shared.NextDouble(),
                CreationDate = DateTime.Now.AddMinutes(12),
                Description = Guid.NewGuid().ToString(),
                IsAvailable = !product.IsAvailable,
                ProducerId = newOwner.Id,
            };
            //act

            Context.ChangeTracker.Clear();

            var result = await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = updateDto
            });

            //assert

            var contextValue = Mapper.Map<ProductDto>(Context.Products.First(x => x.Id == product.Id));

            var prevOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == prevOwner.Email);
            var newOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == newOwner.Email);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(contextValue, Is.Not.EqualTo(product));
                Assert.That(contextValue.Id, Is.EqualTo(updateDto.Id));
                Assert.That(contextValue.Name, Is.EqualTo(updateDto.Name));
                Assert.That(contextValue.Price, Is.EqualTo(updateDto.Price));
                Assert.That(contextValue.CreationDate, Is.EqualTo(updateDto.CreationDate));
                Assert.That(contextValue.IsAvailable, Is.EqualTo(updateDto.IsAvailable));
                Assert.That(contextValue.ProducerId, Is.EqualTo(updateDto.ProducerId));

                Assert.That(newOwnerEmail, Is.Not.Null);
                Assert.That(prevOwnerEmail, Is.Null);
            });
        }

        [Test]
        public async Task UpdateProductComandHandler_ArgumentsValidPrevOwnerEmailNotNullNewOwnerNull_UpdatesValueDoesntSendsEmailToThePrev()
        {
            //arrange
            var newOwner = AuthClient._users.First(u => u.Email == null);
            var prevOwner = AuthClient._users.First(u => u.Email != null && Products.Select(p => p.ProducerId).Contains(u.Id));

            var result1 = await Mediator.Send(new GetProductDtoRequest
            {
                Id = Products.First(p => p.ProducerId == prevOwner.Id).Id,
            });

            ProductDto product = result1.Result ?? throw new Exception();


            Context.ChangeTracker.Clear();


            var updateDto = new UpdateProductDto()
            {
                Id = product.Id,
                Name = Guid.NewGuid().ToString(),
                Price = (decimal)Random.Shared.NextDouble(),
                CreationDate = DateTime.Now.AddMinutes(12),
                Description = Guid.NewGuid().ToString(),
                IsAvailable = !product.IsAvailable,
                ProducerId = newOwner.Id,
            };
            //act

            Context.ChangeTracker.Clear();

            var result = await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = updateDto
            });

            //assert

            var contextValue = Mapper.Map<ProductDto>(Context.Products.First(x => x.Id == product.Id));

            var prevOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == prevOwner.Email);
            var newOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == newOwner.Email);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(contextValue, Is.Not.EqualTo(product));
                Assert.That(contextValue.Id, Is.EqualTo(updateDto.Id));
                Assert.That(contextValue.Name, Is.EqualTo(updateDto.Name));
                Assert.That(contextValue.Price, Is.EqualTo(updateDto.Price));
                Assert.That(contextValue.CreationDate, Is.EqualTo(updateDto.CreationDate));
                Assert.That(contextValue.IsAvailable, Is.EqualTo(updateDto.IsAvailable));
                Assert.That(contextValue.ProducerId, Is.EqualTo(updateDto.ProducerId));

                Assert.That(newOwnerEmail, Is.Null);
                Assert.That(prevOwnerEmail, Is.Not.Null);
            });
        }

        [Test]
        public async Task UpdateProductComandHandler_ArgumentsValidOwnerEmailNotNullNewOwnerEmailNotNull_UpdatesValueSendsEmailsToBoth()
        {
            //arrange
            var newOwner = AuthClient._users.First(u => u.Email != null && !Products.Select(p => p.ProducerId).Contains(u.Id));
            var prevOwner = AuthClient._users.First(u => u.Email != null && Products.Select(p => p.ProducerId).Contains(u.Id));

            var result1 = await Mediator.Send(new GetProductDtoRequest
            {
                Id = Products.First(p => p.ProducerId == prevOwner.Id).Id,
            });

            ProductDto product = result1.Result ?? throw new Exception();


            Context.ChangeTracker.Clear();


            var updateDto = new UpdateProductDto()
            {
                Id = product.Id,
                Name = Guid.NewGuid().ToString(),
                Price = (decimal)Random.Shared.NextDouble(),
                CreationDate = DateTime.Now.AddMinutes(12),
                Description = Guid.NewGuid().ToString(),
                IsAvailable = !product.IsAvailable,
                ProducerId = newOwner.Id,
            };
            //act

            Context.ChangeTracker.Clear();

            var result = await Mediator.Send(new UpdateProductCommand
            {
                UpdateProductDto = updateDto
            });

            //assert

            var contextValue = Mapper.Map<ProductDto>(Context.Products.First(x => x.Id == product.Id));

            var prevOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == prevOwner.Email);
            var newOwnerEmail = EmailSender.Emails.FirstOrDefault(x => x.To == newOwner.Email);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Result, Is.Not.Empty);
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

                Assert.That(contextValue, Is.Not.EqualTo(product));
                Assert.That(contextValue.Id, Is.EqualTo(updateDto.Id));
                Assert.That(contextValue.Name, Is.EqualTo(updateDto.Name));
                Assert.That(contextValue.Price, Is.EqualTo(updateDto.Price));
                Assert.That(contextValue.CreationDate, Is.EqualTo(updateDto.CreationDate));
                Assert.That(contextValue.IsAvailable, Is.EqualTo(updateDto.IsAvailable));
                Assert.That(contextValue.ProducerId, Is.EqualTo(updateDto.ProducerId));

                Assert.That(newOwnerEmail, Is.Not.Null);
                Assert.That(prevOwnerEmail, Is.Not.Null);
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