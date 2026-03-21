namespace WeatherCacheApi.Models;

public class WeatherRecord
{
    public int Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double TemperatureCelsius { get; set; }
    public double FeelsLikeCelsius { get; set; }
    public int Humidity { get; set; }
    public string Description { get; set; } = string.Empty;
    public double WindSpeed { get; set; }
    public DateTime FetchedAt { get; set; }
}
