namespace WeatherCacheApi.Data;

public class DatabaseInitializer
{

    private readonly SqlDbContext _db;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, SqlDbContext db, ILogger<DatabaseInitializer> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        const string sql = """
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'weather'
                    AND TABLE_NAME = 'WeatherRecords'
            )
            BEGIN
                CREATE TABLE [weather].[WeatherRecords] (
                    Id                  INT IDENTITY(1,1) PRIMARY KEY,
                    City                NVARCHAR(100)  NOT NULL,
                    Country             NVARCHAR(10)   NOT NULL,
                    TemperatureCelsius  FLOAT          NOT NULL,
                    FeelsLikeCelsius    FLOAT          NOT NULL,
                    Humidity            INT            NOT NULL,
                    Description         NVARCHAR(255)  NOT NULL,
                    WindSpeed           FLOAT          NOT NULL,
                    FetchedAt           DATETIME2      NOT NULL
                );

                CREATE INDEX IX_WeatherRecords_City_Country_FetchedAt
                    ON [weather].[WeatherRecords] (City, Country, FetchedAt DESC);
            END
            """;

        try
        {
            await _db.ExecuteAsync(sql);
            _logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize the database");
            throw;
        }
    }
}