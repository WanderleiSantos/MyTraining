using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Application.Shared.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> configuration)
    {
        _jwtSettings = configuration.Value;
    }

    public string CreateAccessToken(Guid id, string email)
    {
        return CreateToken(_jwtSettings.Secret, 
            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes), 
            id, 
            email);
    }
    
    public string CreateRefreshToken(Guid id, string email)
    {
        return CreateToken(_jwtSettings.SecretRefresh, 
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpiresDays), 
            id, 
            email);
    }

    public (bool, string?) ValidateRefreshToken(string token)
    {
        return Validate(token, 
            _jwtSettings.SecretRefresh);
    }

    private (bool, string?) Validate(string token, string key)
    {
        var codedKey = Encoding.ASCII.GetBytes(key);
        
        var handler = new JsonWebTokenHandler();
        
        var result = handler.ValidateToken(token, new TokenValidationParameters()
        {
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            RequireSignedTokens = false,
            IssuerSigningKey = new SymmetricSecurityKey(codedKey)
        });
        
        return result.IsValid ? (result.IsValid, result.Claims[JwtRegisteredClaimNames.Email].ToString()) : (false, null);
    }

    private string CreateToken(string secret, DateTime expires, Guid id, string email)
    {
        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = expires,
            NotBefore = DateTime.UtcNow,
            IssuedAt = DateTime.UtcNow,
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}