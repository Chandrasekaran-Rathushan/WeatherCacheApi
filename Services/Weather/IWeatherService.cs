using WeatherCacheApi.Models;

namespace WeatherCacheApi.Services.Weather
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherRecord>> GetAllCachedAsync();
        Task<WeatherRecord?> GetByIdAsync(int id);
        Task<WeatherRecord> GetOrFetchByCityAsync(string city);
    }
}
