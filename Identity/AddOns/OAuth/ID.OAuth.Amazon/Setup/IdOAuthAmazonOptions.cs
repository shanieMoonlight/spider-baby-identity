namespace ID.OAuth.Amazon.Setup;

public class IdOAuthAmazonOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string ApiBaseUrl { get; set; } = "https://api.amazon.com/";
    public int RequestTimeoutSeconds { get; set; } = 30;
}
