namespace BeOnTime.Application.Options;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int ExpirationTimeInMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
