using Microsoft.Data.SqlClient;
using WeatherCacheApi.Data;
using WeatherCacheApi.Models;

namespace WeatherCacheApi.Repositories.Weather
{
    public class WeatherRepository : IWeatherRepository
    {

        private readonly SqlDbContext _db;
        public WeatherRepository(SqlDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WeatherRecord>> GetAllAsync()
        {
            const string sql = """
            SELECT Id, City, Country, TemperatureCelsius, FeelsLikeCelsius,
                   Humidity, Description, WindSpeed, FetchedAt
            FROM WeatherRecords
            ORDER BY FetchedAt DESC
            """;

            return await _db.QueryAsync(sql, MapRow);
        }

        public async Task<WeatherRecord?> GetByCityAsync(string city, string country)
        {
            const string sql = """
            SELECT TOP 1 Id, City, Country, TemperatureCelsius, FeelsLikeCelsius,
                         Humidity, Description, WindSpeed, FetchedAt
            FROM WeatherRecords
            WHERE City = @City
              AND FetchedAt >= DATEADD(HOUR, -1, GETUTCDATE())
            ORDER BY FetchedAt DESC
            """;

            return await _db.QuerySingleOrDefaultAsync(sql, MapRow, [new SqlParameter("@City", city), new SqlParameter("@Country", country)]);
        }

        public async Task<WeatherRecord?> GetByIdAsync(int id)
        {
            const string sql = """
            SELECT Id, City, Country, TemperatureCelsius, FeelsLikeCelsius,
                   Humidity, Description, WindSpeed, FetchedAt
            FROM WeatherRecords
            WHERE Id = @Id
            """;

            return await _db.QuerySingleOrDefaultAsync(sql, MapRow,
            [
                new SqlParameter("@Id", id)
            ]);
        }

        public async Task<WeatherRecord> InsertAsync(WeatherRecord record)
        {
            const string sql = """
            INSERT INTO WeatherRecords
                (City, Country, TemperatureCelsius, FeelsLikeCelsius,
                 Humidity, Description, WindSpeed, FetchedAt)
            OUTPUT INSERTED.Id
            VALUES
                (@City, @Country, @TemperatureCelsius, @FeelsLikeCelsius,
                 @Humidity, @Description, @WindSpeed, @FetchedAt)
            """;

            SqlParameter[] spParams = [
                new SqlParameter("@City",               record.City),
                new SqlParameter("@Country",            record.Country),
                new SqlParameter("@TemperatureCelsius", record.TemperatureCelsius),
                new SqlParameter("@FeelsLikeCelsius",   record.FeelsLikeCelsius),
                new SqlParameter("@Humidity",           record.Humidity),
                new SqlParameter("@Description",        record.Description),
                new SqlParameter("@WindSpeed",          record.WindSpeed),
                new SqlParameter("@FetchedAt",          record.FetchedAt),
            ];

            var newId = await _db.ExecuteScalarAsync<int>(sql, spParams);
            record.Id = newId;
            return record;
        }

        public async Task<IEnumerable<WeatherRecord>> InsertManyAsync(IEnumerable<WeatherRecord> records)
        {
            var list = records.ToList();

            await _db.ExecuteInTransactionAsync(async () =>
            {
                foreach (var record in list)
                    await InsertAsync(record);
            });

            return list;
        }

        private static WeatherRecord MapRow(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            City = r.GetString(1),
            Country = r.GetString(2),
            TemperatureCelsius = r.GetDouble(3),
            FeelsLikeCelsius = r.GetDouble(4),
            Humidity = r.GetInt32(5),
            Description = r.GetString(6),
            WindSpeed = r.GetDouble(7),
            FetchedAt = r.GetDateTime(8)
        };
    }
}
