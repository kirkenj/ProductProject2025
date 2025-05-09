﻿using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediatRExtensions
{
    public class RequestResponseLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class
    {
        private readonly ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> _logger;

        public RequestResponseLoggingBehavior(ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var correlationId = Guid.NewGuid();

            // Request Logging
            // Serialize the request
            var requestJson = JsonSerializer.Serialize(request);
            // Log the serialized request
            _logger.LogInformation("Handling request {CorrelationID}: {Request}", correlationId, requestJson);

            // Response logging
            var response = await next();
            // Serialize the request
            var responseJson = JsonSerializer.Serialize(response);
            // Log the serialized request
            _logger.LogInformation("Response for {Correlation}: {Response}", correlationId, responseJson);

            // Return response
            return response;
        }
    }
}
