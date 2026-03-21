using System.Text.Json.Serialization;

namespace WeatherCacheApi.Models;

public class OpenWeatherResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sys")]
    public OWSys? Sys { get; set; }

    [JsonPropertyName("main")]
    public OWMain? Main { get; set; }

    [JsonPropertyName("weather")]
    public List<OWWeather> Weather { get; set; } = [];

    [JsonPropertyName("wind")]
    public OWWind? Wind { get; set; }
}

public class OWSys
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public class OWMain
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class OWWeather
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class OWWind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}
