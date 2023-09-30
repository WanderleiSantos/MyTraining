using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyTraining.Application.Shared.Configurations;

namespace MyTraining.Application.UseCases.SignIn.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtConfiguration _configuration;

    public AuthenticationService(IOptions<JwtConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public string CreateToken(Guid id, string username)
    {
        var issuer = _configuration.Issuer;
        var audience = _configuration.Audience;
        var key = Encoding.ASCII.GetBytes(_configuration.Key);
        var expires = _configuration.TokenExpires;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(expires),
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, username),
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}