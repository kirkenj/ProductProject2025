using Application.DTOs.Product;
using Application.Features.Product.Requests.Queries;
using ServiceProduct.Tests.Common;
using System.Net;


namespace ServiceProduct.Tests.Product.Queries
{
    public class GeProductHandlerTests : TestBase
    {

        [Test]
        public async Task GetProductDtoRequest_IdDefault_Returns404()
        {
            //arrange

            //act
            var result = await Mediator.Send(new GetProductDtoRequest { Id = default });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetProductDtoRequest_IdNotExcist_Returns404()
        {
            //arrange

            //act
            var result = await Mediator.Send(new GetProductDtoRequest { Id = Guid.NewGuid() });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            });
        }

        [Test]
        public async Task GetProductDtoRequest_IdExcists_ReturnsValue()
        {
            //arrange
            var prods = Products.ToArray();

            var prod = prods[Random.Shared.Next(prods.Length)];

            var expectedResult = Mapper.Map<ProductDto>(prod);

            //act
            var result = await Mediator.Send(new GetProductDtoRequest { Id = prod.Id });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.EqualTo(expectedResult));
                Assert.That(result.Success, Is.True);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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