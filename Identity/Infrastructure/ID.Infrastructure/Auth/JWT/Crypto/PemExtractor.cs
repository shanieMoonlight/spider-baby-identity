namespace ID.Infrastructure.Auth.JWT.Crypto;
internal class PemExtractor
{
    public static string ExtractBase64Payload(string pemString)
    {
        // Remove the header and footer lines
        var lines = pemString.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
        var payloadLines = lines.Skip(1).Take(lines.Length - 2);

        // Concatenate the payload lines and remove any whitespace
        var payload = string.Concat(payloadLines).Replace(" ", "");

        return payload;
    }
}
