using System.Text.Json;
using System.Text.Json.Serialization;

namespace ID.OAuth.Utils.Serialization;

internal class UnixEpochSecondsJsonConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        try
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out long seconds))
                return DateTimeOffset.FromUnixTimeSeconds(seconds);

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (long.TryParse(s, out long secondsStr))
                    return DateTimeOffset.FromUnixTimeSeconds(secondsStr);
            }
        }
        catch
        {
            return null;
        }

        // Unexpected token - skip and return null
        return null;
    }

    //----------------------//

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value.ToUnixTimeSeconds());
        else
            writer.WriteNullValue();
    }

}//Cls
