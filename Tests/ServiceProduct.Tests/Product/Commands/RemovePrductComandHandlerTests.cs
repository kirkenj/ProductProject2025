using Application.Features.Product.Requests.Commands;
using ServiceProduct.Tests.Common;



namespace ServiceProduct.Tests.User.Commands
{
    public class RemovePrductComandHandlerTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            EmailSender.Emails.Clear();
        }


        [Test]
        public async Task RemovePrductComandHandler_IdIsDefault_ThrowsValidationException()
        {
            //arrange

            //act
            var response = await Mediator.Send(new RemovePrductComand { ProductId = default });

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
                Assert.That(response.Message, Is.Not.Empty);
                Assert.That(response.Result, Is.Not.Empty);
            });

        }

        [Test]
        public async Task RemovePrductComandHandler_DtoNotContined_ThrowsValidationException()
        {
            //arrange

            //act
            var response = await Mediator.Send(new RemovePrductComand { ProductId = Guid.NewGuid() });

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
                Assert.That(response.Message, Is.Not.Empty);
                Assert.That(response.Result, Is.Not.Empty);
            });
        }

        [Test]
        public async Task RemovePrductComandHandler_ProducerEmailValid_RemovesValueSendsEmailToProducer()
        {
            //arrange
            var user = AuthClient._users.First(u => u.Email != null);

            var productId = Products.First(p => p.ProducerId == user.Id).Id;


            //act
            var result = await Mediator.Send(new RemovePrductComand
            {
                ProductId = productId
            });

            var lastEmail = EmailSender.LastSentEmail;

            //assert

            var productGetResponse = Products.FirstOrDefault(p => p.Id == productId);

            Assert.Multiple(() =>
            {
                Assert.That(productGetResponse, Is.Null);
                Assert.That(lastEmail, Is.Not.Null);
                Assert.That(lastEmail.To, Is.EqualTo(user.Email));
                Assert.That(lastEmail.Body, Does.Contain(productId.ToString()));
            });
        }

        [Test]
        public async Task RemovePrductComandHandler_ArgumentsValidProducerEmailNull_AddsValueToContextNoEmail()
        {
            //arrange
            var user = AuthClient._users.First(u => u.Email == null);

            var productId = Products.First(p => p.ProducerId == user.Id).Id;


            //act
            var result = await Mediator.Send(new RemovePrductComand
            {
                ProductId = productId
            });

            var lastEmail = EmailSender.LastSentEmail;

            //assert

            var productGetResponse = Products.FirstOrDefault(p => p.Id == productId);

            Assert.Multiple(() =>
            {
                Assert.That(productGetResponse, Is.Null);
                Assert.That(lastEmail, Is.Null);
            });
        }

        [Test]
        public async Task RemovePrductComandHandler_ArgumentsValidOwnerNotExcists_AddsValueToContextNoEmail()
        {
            //arrange

            var productId = Products.First(p => !AuthClient._users.Any(u => u.Id == p.ProducerId)).Id;


            //act
            var result = await Mediator.Send(new RemovePrductComand
            {
                ProductId = productId
            });

            var lastEmail = EmailSender.LastSentEmail;

            //assert

            var productGetResponse = Products.FirstOrDefault(p => p.Id == productId);

            Assert.Multiple(() =>
            {
                Assert.That(productGetResponse, Is.Null);
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