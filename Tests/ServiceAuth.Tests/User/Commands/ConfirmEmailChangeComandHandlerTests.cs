using Application.DTOs.User;
using Application.Features.User.Requests.Commands;
using Application.Models.User;
using Cache.Contracts;
using EmailSender.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence;
using ServiceAuth.Tests.Common;
using Tools;


namespace ServiceAuth.Tests.User.Commands
{
    public class ConfirmEmailChangeComandHandlerTests
    {
        public IMediator Mediator { get; set; } = null!;
        public AuthDbContext Context { get; set; } = null!;
        public IEnumerable<Domain.Models.User> Users => Context.Users;
        public TestEmailSender EmailSender { get; set; } = null!;
        public RedisCustomMemoryCacheWithEvents RedisWithEvents { get; set; } = null!;
        public UpdateUserEmailSettings UpdateUserEmailSettings { get; set; } = null!;


        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.ConfigureTestServices();
            var serviceProvider = services.BuildServiceProvider();
            Mediator = serviceProvider.GetRequiredService<IMediator>();
            Context = serviceProvider.GetRequiredService<AuthDbContext>();
            Context.Database.EnsureCreated();
            if (serviceProvider.GetRequiredService<IEmailSender>() is not TestEmailSender tes) throw new Exception();
            EmailSender = tes;
            if (serviceProvider.GetRequiredService<ICustomMemoryCache>() is not RedisCustomMemoryCacheWithEvents rwe) throw new Exception();
            RedisWithEvents = rwe;
            UpdateUserEmailSettings = serviceProvider.GetRequiredService<IOptions<UpdateUserEmailSettings>>().Value;
            Context.ChangeTracker.Clear();
            EmailSender.Emails.Clear();
        }

        [Test]
        public void ConfirmEmailChangeComandHandler_ConfirmEmailChangeDtoIsNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new ConfirmEmailChangeComand { ConfirmEmailChangeDto = null });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ConfirmEmailChangeComandHandler_ConfirmEmailChangeDtoNotSet_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new ConfirmEmailChangeComand { ConfirmEmailChangeDto = new() });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ConfirmEmailChangeComandHandler_ArgumentsAreNull_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new ConfirmEmailChangeComand
            {
                ConfirmEmailChangeDto = new()
                {
                    Token = null,
                    Id = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void ConfirmEmailChangeComandHandler_ArgumentsAreEmpty_ThrowsValidationException()
        {
            //arrange

            //act
            var func = async () => await Mediator.Send(new ConfirmEmailChangeComand
            {
                ConfirmEmailChangeDto = new()
                {
                    Token = string.Empty,
                    Id = default
                }
            });

            //assert
            Assert.That(func, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task ConfirmEmailChangeComandHandler_TokenInvalid_CacheReturnsNullReturnsBadRequest()
        {
            //arrange
            var newEmail = Random.Shared.Next().ToString() + "someEmail@meow";
            var useroTokenFor = Users.First();
            var userForInvalidConfirmRequest = Users.Last();

            var clonedUseroTokenFor = JsonCloner.Clone(useroTokenFor);
            var clonedUserForInvalidConfirmRequest = JsonCloner.Clone(userForInvalidConfirmRequest);


            var requestCreatonDto = new UpdateUserEmailDto { Email = newEmail, Id = useroTokenFor.Id };
            await Mediator.Send(new SendTokenToUpdateUserEmailRequest { UpdateUserEmailDto = requestCreatonDto });


            var lastMessage = EmailSender.LastSentEmail;
            if (lastMessage.To != newEmail) throw new Exception("EmailSender.LastSentEmail.To != newEmail");

            var confirmToken = Guid.NewGuid().ToString();

            var keyToTrack = string.Format(UpdateUserEmailSettings.UpdateUserEmailCacheKeyFormat, confirmToken);
            bool keyToTrackGetInvoked = false;
            UpdateUserEmailDto? cachedValueOnKey = null;

            RedisWithEvents.OnGet += (key, value) =>
            {
                if (key == keyToTrack)
                {
                    keyToTrackGetInvoked = true;
                    cachedValueOnKey = value is UpdateUserEmailDto dtoVal ? dtoVal : cachedValueOnKey;
                }
            };

            //act
            var result = await Mediator.Send(new ConfirmEmailChangeComand
            {
                ConfirmEmailChangeDto = new()
                {
                    Token = confirmToken,
                    Id = useroTokenFor.Id
                }
            });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, $"{nameof(result.Success)} has to be fals");
                Assert.That(result.Result, Is.Null, $"{nameof(result.Result)} has to be null");
                Assert.That(result.Message, Is.Not.Empty, $"{nameof(result.Message)} has to be not empty");
                Assert.That(keyToTrackGetInvoked, Is.True, $"{nameof(RedisWithEvents.OnGet)} event was not invoked with key {keyToTrack}");
                Assert.That(cachedValueOnKey, Is.Null, $"{nameof(cachedValueOnKey)} has to be null");
            });
        }

        [Test]
        public async Task ConfirmEmailChangeComandHandler_TokenValidUserRemovedAfterTokenSent_ReturnsBadRequest()
        {
            //arrange
            var newEmail = Random.Shared.Next().ToString() + "someEmail@meow";
            var useroTokenFor = Users.First();

            var clonedUseroTokenFor = JsonCloner.Clone(useroTokenFor);


            var requestCreatonDto = new UpdateUserEmailDto { Email = newEmail, Id = useroTokenFor.Id };
            await Mediator.Send(new SendTokenToUpdateUserEmailRequest { UpdateUserEmailDto = requestCreatonDto });


            var lastMessage = EmailSender.LastSentEmail;
            if (lastMessage.To != newEmail) throw new Exception("EmailSender.LastSentEmail.To != newEmail");

            var confirmToken = lastMessage.Body.ParseExact(UpdateUserEmailSettings.UpdateUserEmailMessageBodyFormat)[0];

            var keyToTrack = string.Format(UpdateUserEmailSettings.UpdateUserEmailCacheKeyFormat, confirmToken);
            bool keyToTrackGetInvoked = false;
            UpdateUserEmailDto? cachedValueOnKey = null;

            RedisWithEvents.OnGet += (key, value) =>
            {
                if (key == keyToTrack)
                {
                    keyToTrackGetInvoked = true;
                    cachedValueOnKey = value is UpdateUserEmailDto dtoVal ? dtoVal : cachedValueOnKey;
                }
            };

            //act
            Context.Remove(useroTokenFor);
            await Context.SaveChangesAsync();

            var result = await Mediator.Send(new ConfirmEmailChangeComand
            {
                ConfirmEmailChangeDto = new()
                {
                    Token = confirmToken,
                    Id = useroTokenFor.Id
                }
            });

            //assert
            Assert.Multiple(() =>
            {

                Assert.That(result.Success, Is.False, $"{nameof(result.Success)} has to be false");
                Assert.That(result.Result, Is.Null, $"{nameof(result.Result)} has to be null");
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), $"{nameof(result.StatusCode)} has to be {System.Net.HttpStatusCode.BadRequest}");
                Assert.That(result.Message, Is.Not.Empty);
                Assert.That(keyToTrackGetInvoked, Is.True, $"{nameof(RedisWithEvents.OnGet)} event was not invoked with key {keyToTrack}");
                Assert.That(cachedValueOnKey, Is.Not.Null, $"{nameof(cachedValueOnKey)} has to be null");
            });
        }



        [Test]
        public async Task ConfirmEmailChangeComandHandler_TokenValidUserIdIsNotEqualToTheCachedUpdateRequestUserId_FindsOutOfWrongIdReturnsBadRequest()
        {
            //arrange
            var newEmail = Random.Shared.Next().ToString() + "someEmail@meow";
            var useroTokenFor = Users.First();
            var userForInvalidConfirmRequest = Users.Last();

            var clonedUseroTokenFor = JsonCloner.Clone(useroTokenFor);
            var clonedUserForInvalidConfirmRequest = JsonCloner.Clone(userForInvalidConfirmRequest);


            var requestCreatonDto = new UpdateUserEmailDto { Email = newEmail, Id = useroTokenFor.Id };
            await Mediator.Send(new SendTokenToUpdateUserEmailRequest { UpdateUserEmailDto = requestCreatonDto });


            var lastMessage = EmailSender.LastSentEmail;
            if (lastMessage.To != newEmail) throw new Exception("EmailSender.LastSentEmail.To != newEmail");

            var confirmToken = lastMessage.Body.ParseExact(UpdateUserEmailSettings.UpdateUserEmailMessageBodyFormat)[0];

            var keyToTrack = string.Format(UpdateUserEmailSettings.UpdateUserEmailCacheKeyFormat, confirmToken);
            bool keyToTrackGetInvoked = false;
            bool keyToTrackRemoveInvoked = false;
            UpdateUserEmailDto? cachedValueOnKey = null;

            RedisWithEvents.OnGet += (key, value) =>
            {
                if (key == keyToTrack)
                {
                    keyToTrackGetInvoked = true;
                    cachedValueOnKey = value is UpdateUserEmailDto dtoVal ? dtoVal : cachedValueOnKey;
                }
            };

            RedisWithEvents.OnRemove += (key) =>
            {
                keyToTrackRemoveInvoked = key == keyToTrack;
            };

            //act
            var result = await Mediator.Send(new ConfirmEmailChangeComand
            {
                ConfirmEmailChangeDto = new()
                {
                    Token = confirmToken,
                    Id = userForInvalidConfirmRequest.Id
                }
            });

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, $"{nameof(result.Success)} has to be false");
                Assert.That(result.Result, Is.Null, $"{nameof(result.Result)} has to be null");
                Assert.That(result.Message, Is.Not.Empty, $"{nameof(result.Message)} has to be not empty");
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), $"{nameof(result.StatusCode)} has to be {System.Net.HttpStatusCode.BadRequest}");
                Assert.That(keyToTrackGetInvoked, Is.True, $"Event {nameof(RedisWithEvents.OnGet)} with key '{keyToTrack}' was not invoked");
                Assert.That(keyToTrackRemoveInvoked, Is.False, $"Event {nameof(RedisWithEvents.OnRemove)} with key '{keyToTrack}' was invoked");
                Assert.That(cachedValueOnKey, Is.EqualTo(requestCreatonDto));

                Assert.That(useroTokenFor, Is.EqualTo(clonedUseroTokenFor), $"{nameof(useroTokenFor)} mustn't be upadted after invalid command");
                Assert.That(userForInvalidConfirmRequest, Is.EqualTo(clonedUserForInvalidConfirmRequest), $"{nameof(userForInvalidConfirmRequest)} mustn't be upadted after invalid command");
            });
        }

        [Test]
        public async Task ConfirmEmailChangeComandHandler_ArgumentsAreValid_GetsCachedValueUpdatesValueInContextRemovesCacheKey()
        {
            //arrange
            var newEmail = Random.Shared.Next().ToString() + "someEmail@meow";
            var user = Users.First();

            var mayBeModifiedFields = new string[] { nameof(user.Email) };

            var clonedUserBeforeUpdate = JsonCloner.Clone(user);

            var testRequestDto = new UpdateUserEmailDto { Email = newEmail, Id = user.Id };
            await Mediator.Send(new SendTokenToUpdateUserEmailRequest { UpdateUserEmailDto = testRequestDto });

            var lastMessage = EmailSender.LastSentEmail;
            if (lastMessage.To != newEmail) throw new Exception("EmailSender.LastSentEmail.To != newEmail");

            var confirmToken = lastMessage.Body.ParseExact(UpdateUserEmailSettings.UpdateUserEmailMessageBodyFormat)[0];

            var keyToTrack = string.Format(UpdateUserEmailSettings.UpdateUserEmailCacheKeyFormat, confirmToken);
            bool keyToTrackGetInvoked = false;
            bool keyToTrackRemoveInvoked = false;
            UpdateUserEmailDto? cachedValueOnKey = null;

            RedisWithEvents.OnGet += (key, value) =>
            {
                if (key == keyToTrack)
                {
                    keyToTrackGetInvoked = true;
                    cachedValueOnKey = value is UpdateUserEmailDto dtoVal ? dtoVal : cachedValueOnKey;
                }
            };

            RedisWithEvents.OnRemove += (key) =>
            {
                keyToTrackRemoveInvoked = key == keyToTrack;
            };

            //act
            var result = await Mediator.Send(new ConfirmEmailChangeComand
            {
                ConfirmEmailChangeDto = new()
                {
                    Token = confirmToken,
                    Id = user.Id
                }
            });

            //assert

            var cmpResult = FieldComparator.GetNotEqualPropertiesAndFieldsNames(clonedUserBeforeUpdate, user, nameof(user.Role));

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True, $"{nameof(result.Success)} has to be true");
                Assert.That(result.Result, Is.Not.Empty, $"{nameof(result.Result)} has to be not emty");
                Assert.That(result.Message, Is.Empty, $"{nameof(result.Message)} has to be empty");
                Assert.That(result.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), $"{nameof(result.StatusCode)} has to be {System.Net.HttpStatusCode.OK}");
                Assert.That(keyToTrackGetInvoked, Is.True, $"Event {nameof(RedisWithEvents.OnGet)} with key '{keyToTrack}' was not invoked");
                Assert.That(keyToTrackRemoveInvoked, Is.True, $"Event {nameof(RedisWithEvents.OnRemove)} with key '{keyToTrack}' was not invoked");
                Assert.That(cachedValueOnKey, Is.EqualTo(testRequestDto));

                Assert.That(cmpResult, Is.SubsetOf(mayBeModifiedFields), $"Only these fields and properties may be modified: ({string.Join(", ", mayBeModifiedFields)})");

                Assert.That(user.Email, Is.EqualTo(newEmail), $"Parameter {nameof(user.Email)} has to be the same as email in the request");
                Assert.That(user.Email, Is.EqualTo(cachedValueOnKey.Email), $"Parameter {nameof(user.Email)} has to be the same email in cached request");
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