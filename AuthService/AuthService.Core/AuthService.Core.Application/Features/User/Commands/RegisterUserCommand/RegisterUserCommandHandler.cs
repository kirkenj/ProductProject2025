﻿using Application.Models.User;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Models.User.Settings;
using AutoMapper;
using Cache.Contracts;
using CustomResponse;
using MediatR;
using Messaging.Kafka.Producer.Contracts;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Options;

namespace AuthService.Core.Application.Features.User.Commands.RegisterUserCommand
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Response<string>>
    {
        private readonly IPasswordSetter _passwordSetter;
        private readonly CreateUserSettings _createUserSettings;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IKafkaProducer<UserRegistrationRequestCreated> _userRegistrationRequestCreatedKafkaProducer;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IOptions<CreateUserSettings> createUserSettings,
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordGenerator passwordGenerator,
            ICustomMemoryCache memoryCache,
            IKafkaProducer<UserRegistrationRequestCreated> kafkaProducer,
            IPasswordSetter passwordSetter)
        {
            _createUserSettings = createUserSettings.Value;
            _passwordGenerator = passwordGenerator;
            _passwordSetter = passwordSetter;
            _memoryCache = memoryCache;
            _userRegistrationRequestCreatedKafkaProducer = kafkaProducer;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetAsync(new UserFilter { AccurateEmail = request.Email }, cancellationToken) != null)
            {
                return Response<string>.BadRequestResponse("Email is taken");
            }

            Domain.Models.User user = _mapper.Map<Domain.Models.User>(request);

            user.Id = Guid.NewGuid();
            user.Login = $"User{user.Id}";
            user.RoleID = _createUserSettings.DefaultRoleID;

            _passwordSetter.SetPassword(request.Password, user);

            var token = _passwordGenerator.Generate();

            var cacheKey = string.Format(_createUserSettings.KeyForRegistrationCachingFormat, user.Email, token);

            await _memoryCache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(_createUserSettings.ConfirmationTimeout), cancellationToken);

            await _userRegistrationRequestCreatedKafkaProducer.ProduceAsync(new()
            {
                Email = user.Email,
                Token = token,
            }, cancellationToken);

            return Response<string>.OkResponse($"Created user registration request. Further details sent on email", string.Empty);
        }
    }
}