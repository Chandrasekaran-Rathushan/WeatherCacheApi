using Microsoft.AspNetCore.Mvc;
using WeatherCacheApi.Models;
using WeatherCacheApi.Services.Weather;

namespace WeatherCacheApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // GET: api/weather
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherRecord>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WeatherRecord>>> GetAll()
        {
            var records = await _weatherService.GetAllCachedAsync();
            return Ok(records);
        }

        // GET: api/weather/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(WeatherRecord), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WeatherRecord>> GetById(int id)
        {
            var record = await _weatherService.GetByIdAsync(id);
            return record is null ? NotFound(new { error = $"Record with ID {id} not found." }) : Ok(record);
        }

        // GET: api/weather/city/{city}
        [HttpGet("city/{city}")]
        [ProducesResponseType(typeof(WeatherRecord), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<ActionResult<WeatherRecord>> GetByCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return BadRequest(new { error = "City name must not be empty." });

            var record = await _weatherService.GetOrFetchByCityAsync(city);
            return Ok(record);
        }
    }
}
