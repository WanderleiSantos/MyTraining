namespace Application.Shared.Authentication;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 60;
    public string SecretRefresh { get; set; } = string.Empty;
    public int RefreshExpiresDays { get; set; } = 30;
}