using ID.OAuth.Utils.Serialization;

namespace ID.OAuth.Facebook.Tests;

public class UnixEpochSecondsJsonConverterTests
{
    private static JsonSerializerOptions CreateOptionsWithConverter()
    {
        var opts = new JsonSerializerOptions();

        opts.Converters.Add(new UnixEpochSecondsJsonConverter());
        //// create converter instance via reflection because the converter is internal to the addon assembly
        //var convType = Type.GetType("ID.OAuth.Facebook.Data.UnixEpochSecondsJsonConverter, ID.OAuth.Facebook");
        //convType.ShouldNotBeNull();
        //var convObj = Activator.CreateInstance(convType, nonPublic: true);
        //convObj.ShouldNotBeNull();
        //opts.Converters.Add((System.Text.Json.Serialization.JsonConverter)convObj);
        return opts;
    }

    //----------------------//


    [Fact]
    public void Read_NumberToken_ReturnsDateTimeOffset()
    {
        var options = CreateOptionsWithConverter();
        var seconds = 1670000000L;
        var json = seconds.ToString();

        var dto = JsonSerializer.Deserialize<DateTimeOffset?>(json, options);
        dto.ShouldNotBeNull();
        dto.Value.ToUnixTimeSeconds().ShouldBe(seconds);
    }

    //----------------------//


    [Fact]
    public void Read_StringNumberToken_ReturnsDateTimeOffset()
    {
        var options = CreateOptionsWithConverter();
        var seconds = 1670000000L;
        var json = '"' + seconds.ToString() + '"';

        var dto = JsonSerializer.Deserialize<DateTimeOffset?>(json, options);
        dto.ShouldNotBeNull();
        dto.Value.ToUnixTimeSeconds().ShouldBe(seconds);
    }

    //----------------------//


    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var options = CreateOptionsWithConverter();
        var dto = JsonSerializer.Deserialize<DateTimeOffset?>("null", options);
        dto.ShouldBeNull();
    }

    //----------------------//


    [Fact]
    public void Read_MalformedString_ReturnsNull()
    {
        var options = CreateOptionsWithConverter();
        var dto = JsonSerializer.Deserialize<DateTimeOffset?>("\"notanumber\"", options);
        dto.ShouldBeNull();
    }

    //----------------------//


    [Fact]
    public void Write_WritesNumberSeconds()
    {
        var options = CreateOptionsWithConverter();
        var seconds = 1670000000L;
        var dto = DateTimeOffset.FromUnixTimeSeconds(seconds);

        var json = JsonSerializer.Serialize<DateTimeOffset?>(dto, options);
        // Expect a JSON number
        json.ShouldBe(seconds.ToString());
    }

}//Cls
