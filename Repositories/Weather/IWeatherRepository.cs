using WeatherCacheApi.Models;

namespace WeatherCacheApi.Repositories.Weather
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<WeatherRecord>> GetAllAsync();
        Task<WeatherRecord?> GetByIdAsync(int id);
        Task<WeatherRecord?> GetByCityAsync(string city, string country);
        Task<WeatherRecord> InsertAsync(WeatherRecord record);
        Task<IEnumerable<WeatherRecord>> InsertManyAsync(IEnumerable<WeatherRecord> records); // insert multiple records
    }
}
