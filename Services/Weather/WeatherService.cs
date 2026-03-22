using WeatherCacheApi.Models;
using WeatherCacheApi.Repositories.Weather;
using WeatherCacheApi.Services.OpenWeatherService;

namespace WeatherCacheApi.Services.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _repo;
        private readonly IOpenWeatherService _owService;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(
            IWeatherRepository repo,
            IOpenWeatherService owService,
            ILogger<WeatherService> logger)
        {
            _repo = repo;
            _owService = owService;
            _logger = logger;
        }
        public async Task<IEnumerable<WeatherRecord>> GetAllCachedAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<WeatherRecord?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<WeatherRecord> GetOrFetchByCityAsync(string city)
        {
            var cityNorm = city.Trim();

            var cached = await FindCachedByCityNameAsync(cityNorm);

            if (cached is not null)
            {
                _logger.LogInformation("Cache found for city '{City}' (Id={Id})", cityNorm, cached.Id);
                return cached;
            }

            _logger.LogInformation("Cache is missing for city '{City}', fetching from API...", cityNorm);
            var record = await _owService.FetchWeatherAsync(cityNorm);

            return await _repo.InsertAsync(record);
        }

        private async Task<WeatherRecord?> FindCachedByCityNameAsync(string city)
        {
            return await _repo.GetByCityAsync(city, string.Empty)
                ?? await TryFindByPartialAsync(city);
        }

        private async Task<WeatherRecord?> TryFindByPartialAsync(string city)
        {
            // Fetch all cached and find a city name match
            var all = await _repo.GetAllAsync();
            return all.FirstOrDefault(r =>
                string.Equals(r.City, city, StringComparison.OrdinalIgnoreCase) &&
                r.FetchedAt >= DateTime.UtcNow.AddHours(-1));
        }
    }
}
