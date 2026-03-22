using System.Text.Json;
using WeatherCacheApi.Models;

namespace WeatherCacheApi.Services.OpenWeatherService
{
    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly HttpClient _http;
        private readonly string _openWeatherBaseUrl;
        private readonly string _apiKey;
        private readonly ILogger<OpenWeatherService> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public OpenWeatherService(HttpClient http, IConfiguration configuration, ILogger<OpenWeatherService> logger)
        {
            _http = http;
            _logger = logger;

            _openWeatherBaseUrl = configuration["OpenWeather:BaseUrl"] ?? throw new InvalidOperationException("OpenWeather:ApiKey is not found.");
            _apiKey = configuration["OpenWeather:ApiKey"] ?? throw new InvalidOperationException("OpenWeather:ApiKey is not found.");
        }

        public async Task<WeatherRecord> FetchWeatherAsync(string city)
        {
            var url = $"{_openWeatherBaseUrl}/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";

            _logger.LogInformation("Fetching weather from OpenWeatherMap for city: {City}", city);

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("OpenWeatherMap returned {Status} for city {City}: {Body}", response.StatusCode, city, body);

                throw response.StatusCode == System.Net.HttpStatusCode.NotFound
                    ? new KeyNotFoundException($"City '{city}' was not found in OpenWeatherMap.")
                    : new HttpRequestException($"OpenWeatherMap error ({response.StatusCode}): {body}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var ow = JsonSerializer.Deserialize<OpenWeatherResponse>(json, _jsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize response OpenWeatherMap");

            return new WeatherRecord
            {
                City = ow.Name,
                Country = ow.Sys?.Country ?? string.Empty,
                TemperatureCelsius = ow.Main?.Temp ?? 0,
                FeelsLikeCelsius = ow.Main?.FeelsLike ?? 0,
                Humidity = ow.Main?.Humidity ?? 0,
                Description = ow.Weather.FirstOrDefault()?.Description ?? string.Empty,
                WindSpeed = ow.Wind?.Speed ?? 0,
                FetchedAt = DateTime.UtcNow
            };
        }
    }
}
