using System.Text;
using System.Text.Json.Serialization;

namespace ID.OAuth.Facebook.Data;


/// <summary>
/// Internal model for Facebook Graph API user profile response
/// </summary>
public class FacebookUserProfile
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("picture")]
    public FacebookPictureData? Picture { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("timezone")]
    public int? Timezone { get; set; }

    [JsonPropertyName("birthday")]
    public string? Birthday { get; set; } // Format: MM/DD/YYYY

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }   // Add this

    //----------------------//

    public override string ToString() =>
        new StringBuilder()
            .AppendLine("FacebookUserProfile:")
            .AppendLine($"Id: {Id ?? string.Empty}")
            .AppendLine($"Email: {Email ?? string.Empty}")
            .AppendLine($"Name: {Name ?? string.Empty}")
            .AppendLine($"FirstName: {FirstName ?? string.Empty}")
            .AppendLine($"LastName: {LastName ?? string.Empty}")
            .AppendLine($"Gender: {Gender ?? string.Empty}")
            .AppendLine($"Locale: {Locale ?? string.Empty}")
            .AppendLine($"Timezone: {(Timezone.HasValue ? Timezone.Value.ToString() : string.Empty)}")
            .AppendLine($"Birthday: {Birthday ?? string.Empty}")
            .AppendLine($"Verified: {Verified}")
            .AppendLine($"Picture: {(Picture?.Data != null ? Picture.Data.ToString() : string.Empty)}")
            .ToString();
}


//###########################################//

/// <summary>
/// Internal model for Facebook profile picture data
/// </summary>
public class FacebookPictureData
{
    [JsonPropertyName("data")]
    public FacebookPictureUrl? Data { get; set; }

    //------------------------//

    public override string ToString() =>
        new StringBuilder()
          .AppendLine($"Data:")
          .AppendLine($"{Data}")
          .ToString();

}


//###########################################//

/// <summary>
/// Internal model for Facebook profile picture URL
/// </summary>
public class FacebookPictureUrl
{
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("is_silhouette")]
    public bool IsSilhouette { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    //----------------------//

    public override string ToString() =>
        new StringBuilder()
            .AppendLine("FacebookPictureUrl:")
            .AppendLine($"Url: {Url ?? string.Empty}")
            .AppendLine($"IsSilhouette: {IsSilhouette}")
            .AppendLine($"Width: {Width}")
            .AppendLine($"Height: {Height}")
            .ToString();
}