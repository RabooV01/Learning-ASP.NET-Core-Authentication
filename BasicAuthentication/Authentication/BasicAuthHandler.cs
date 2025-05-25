using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BasicAuthentication.Authentication;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
        
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        string authHeader = Request.Headers["Authorization"].ToString();
        if(!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Auth type is invalid"));
        }

        // username: creds[0], password: creds[1]
        string authCreds = authHeader["Basic ".Length..];
        var creds = Encoding.UTF8.GetString(Convert.FromBase64String(authCreds)).Split(':');

        if(creds is null || creds.Length < 2)
        {
            System.Console.WriteLine(creds);
            return Task.FromResult(AuthenticateResult.Fail("Auth format is incorrect"));
        }

        if(!creds[0].Equals("rabii@gmail.com") || !creds[1].Equals("asd123asd"))
        {
            return Task.FromResult(AuthenticateResult.Fail("invalid credentials"));
        }

        var claimsIdentity = new ClaimsIdentity(new Claim[]{
            new Claim(ClaimTypes.NameIdentifier, Guid.CreateVersion7().ToString()),
            new Claim(ClaimTypes.Name, "rabii")
        }, "Basic");

        var userPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authTicket = new AuthenticationTicket(userPrincipal, "Basic");
        
        return Task.FromResult(AuthenticateResult.Success(authTicket));
    }
}