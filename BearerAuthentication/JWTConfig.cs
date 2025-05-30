namespace BearerAuthentication;
public class JWTConfig 
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Expiration { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;

}