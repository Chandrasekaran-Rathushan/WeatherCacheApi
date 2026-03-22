using WeatherCacheApi.Models;

namespace WeatherCacheApi.Repositories.OpenWeatherService
{
    public interface IOpenWeatherService
    {
        Task<WeatherRecord> FetchWeatherAsync(string city);
    }
}
