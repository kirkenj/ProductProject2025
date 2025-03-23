namespace Clients.CustomGateway
{
    public partial interface IAuthGatewayClient
    {
        /// <returns>Success</returns>
        /// <exception cref="AuthGatewayClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.String> RegisterAsync(CreateUserDto body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="AuthGatewayClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.String> RegisterAsync(CreateUserDto body, System.Threading.CancellationToken cancellationToken);

        /// <returns>Success</returns>
        /// <exception cref="AuthGatewayClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<LoginResultModel> LoginPOSTAsync(LoginDto body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="AuthGatewayClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<LoginResultModel> LoginPOSTAsync(LoginDto body, System.Threading.CancellationToken cancellationToken);

        /// <returns>Success</returns>
        /// <exception cref="AuthGatewayClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> ForgotPasswordAsync(string body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Success</returns>
        /// <exception cref="AuthGatewayClientException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> ForgotPasswordAsync(string body, System.Threading.CancellationToken cancellationToken);
    }
}