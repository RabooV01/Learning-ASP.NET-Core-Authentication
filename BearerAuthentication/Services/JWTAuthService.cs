using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BearerAuthentication.Services;

public class JWTAuthService 
{
    private readonly JWTOptions _options;
    private readonly List<User> _usersRepository; // lets say its our user repository

    public JWTAuthService(JWTOptions options, List<User> usersRepository)
    {
        _options = options;
        _usersRepository = usersRepository;
    }

    public string AuthenticateUser(Login login)
    {
        var user = _usersRepository.FirstOrDefault(usr => usr.username == login.username && usr.password == login.password);
        
        if(user == null)
        {
            throw new Exception("Invalid username or password");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var tokenDiscriptor = new SecurityTokenDescriptor() 
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_options.Expiration),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.username)
            })
        };

        var token = tokenHandler.CreateToken(tokenDiscriptor);
        
        return tokenHandler.WriteToken(token);
    }
}