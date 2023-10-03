using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Shared.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Shared.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtConfiguration _configuration;

    public AuthenticationService(IOptions<JwtConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public string CreateAccessToken(Guid id, string username)
    {
        return CreateToken(_configuration.Key, _configuration.TokenExpires, id, username);
    }
    
    public string CreateRefreshToken(Guid id, string username)
    {
        return CreateToken(_configuration.Key, _configuration.TokenExpires, id, username);
    }

    private string CreateToken(string keyConfig, int expires, Guid id, string username)
    {
        var issuer = _configuration.Issuer;
        var audience = _configuration.Audience;
        var key = Encoding.ASCII.GetBytes(keyConfig);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(expires),
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(new[]
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