using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Shared.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

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
        return CreateToken(_configuration.Refresh, _configuration.RefreshExpires, id, username);
    }

    public (bool, string?) ValidateRefreshToken(string token)
    {
        return Validate(token, _configuration.Refresh);
    }

    private (bool, string?) Validate(string token, string key)
    {
        var codedKey = Encoding.ASCII.GetBytes(key);
        
        var handler = new JsonWebTokenHandler();
        
        var result = handler.ValidateToken(token, new TokenValidationParameters()
        {
            ValidIssuer = _configuration.Issuer,
            ValidAudience = _configuration.Audience,
            RequireSignedTokens = false,
            IssuerSigningKey = new SymmetricSecurityKey(codedKey)
        });
        
        return result.IsValid ? (result.IsValid, result.Claims[JwtRegisteredClaimNames.Email].ToString()) : (false, null);
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
                new Claim(JwtRegisteredClaimNames.NameId, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, username),
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}