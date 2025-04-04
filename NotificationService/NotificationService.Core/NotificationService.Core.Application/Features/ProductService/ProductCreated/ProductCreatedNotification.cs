﻿using System.Text.Json.Serialization;
using Clients.AuthApi;
using Clients.ProductService.Clients.ProductServiceClient;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated
{
    public class ProductCreatedNotification : IMediatRSendableNotification
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        [JsonIgnore]
        public ProductDto ProductDto { get; set; } = null!;
        [JsonIgnore]
        public UserDto UserDto { get; set; } = null!;
    }
}
