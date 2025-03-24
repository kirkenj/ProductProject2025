namespace CentralizedJwtAuthentication
{
    public class JwtSettings
    {
        public string IssuerTokenValidatePostMethodUri { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty!;
        public double DurationInMinutes { get; set; }
    }
}
