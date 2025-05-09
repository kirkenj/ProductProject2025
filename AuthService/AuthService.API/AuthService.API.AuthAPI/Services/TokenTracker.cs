﻿using AuthAPI.Models.TokenTracker;
using AuthService.API.AuthAPI.Contracts;
using Cache.Contracts;
using Microsoft.Extensions.Options;

namespace AuthService.API.AuthAPI.Services
{
    public class TokenTracker<TUserIdType> : ITokenTracker<TUserIdType> where TUserIdType : struct
    {
        private readonly TokenTrackingSettings _settings = null!;
        private readonly ICustomMemoryCache _memoryCache;
        private readonly IJwtProviderService _jwtProviderService;
        private readonly Func<string, string> _keyGeneratingDelegate;

        public TokenTracker(IOptions<TokenTrackingSettings> options, ICustomMemoryCache memoryCache, IJwtProviderService jwtProviderService)
        {
            _settings = options.Value;
            _memoryCache = memoryCache;
            _jwtProviderService = jwtProviderService;
            _keyGeneratingDelegate = (value) => _settings.CacheSeed + value;
        }

        public async Task InvalidateUser(TUserIdType userId, DateTime time)
        {
            if (userId.Equals(default))
            {
                throw new ArgumentException($"{nameof(userId)} can not be {default(TUserIdType)}", nameof(userId));
            }

            string userIdStr = userId.ToString() ?? throw new ApplicationException("Couldn't get user's id as string");

            await _memoryCache.SetAsync(
                _keyGeneratingDelegate(userIdStr),
                time,
                TimeSpan.FromMinutes(_settings.DurationInMinutes));

            return;
        }

        public async Task<bool> IsValid(string token)
        {
            if (string.IsNullOrEmpty(token) || !_jwtProviderService.IsTokenValid(token))
            {
                return false;
            }

            var key = _keyGeneratingDelegate(token);
            var trackInfo = await _memoryCache.GetAsync<AssignedTokenInfo<TUserIdType>>(key);

            if (trackInfo == null)
            {
                return false;
            }

            if (trackInfo.UserId.Equals(default))
            {
                throw new InvalidOperationException(nameof(trackInfo));
            }

            var banResult = await _memoryCache.GetAsync<DateTime>(
                _keyGeneratingDelegate(trackInfo.UserId.ToString() ?? throw new InvalidOperationException(nameof(trackInfo))));

            return banResult == default || trackInfo.DateTime > banResult;
        }

        public async Task Track(string token, TUserIdType userId, DateTime tokenRegistrationTime)
        {
            var cahceKey = _keyGeneratingDelegate(token);
            await _memoryCache.SetAsync(
                cahceKey,
                new AssignedTokenInfo<TUserIdType> { DateTime = tokenRegistrationTime, UserId = userId },
                TimeSpan.FromMinutes(_settings.DurationInMinutes
                ));
        }
    }
}
