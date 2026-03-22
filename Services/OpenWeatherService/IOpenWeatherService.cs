using WeatherCacheApi.Models;

namespace WeatherCacheApi.Services.OpenWeatherService
{
    public interface IOpenWeatherService
    {
        Task<WeatherRecord> FetchWeatherAsync(string city);
    }
}
