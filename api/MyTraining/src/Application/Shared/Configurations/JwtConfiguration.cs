namespace Application.Shared.Configurations;

public class JwtConfiguration
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Refresh { get; set; } = string.Empty;
    public int TokenExpires { get; set; } = 60;
    public int RefreshExpires { get; set; } = 30;
}