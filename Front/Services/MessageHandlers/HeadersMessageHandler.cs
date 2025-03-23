namespace Front.Services.MessageHandlers
{
    public class HeadersMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Cors headers attached");

            _ = request.Headers.Append(new KeyValuePair<string, IEnumerable<string>>("Access-Control-Allow-Origin", ["*"]));
            _ = request.Headers.Append(new KeyValuePair<string, IEnumerable<string>>("Access-Control-Allow-Credentials", ["true"]));
            _ = request.Headers.Append(new KeyValuePair<string, IEnumerable<string>>("Access-Control-Allow-Headers", ["Content-Type"]));

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
