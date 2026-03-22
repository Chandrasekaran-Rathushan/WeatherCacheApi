USE [WeatherCache]
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'weather'
      AND TABLE_NAME = 'WeatherRecords'
)
BEGIN
    CREATE TABLE [weather].[WeatherRecords] (
        Id                      INT IDENTITY(1,1) PRIMARY KEY,
        City                    NVARCHAR(100)  NOT NULL,
        Country                 NVARCHAR(10)   NOT NULL,
        TemperatureCelsius      FLOAT          NOT NULL,
        FeelsLikeCelsius        FLOAT          NOT NULL,
        Humidity                INT            NOT NULL,
        [Description]           NVARCHAR(255)  NOT NULL,
        WindSpeed               FLOAT          NOT NULL,
        FetchedAt               DATETIME2      NOT NULL
    );

    CREATE INDEX IX_WeatherRecords_City_Country_FetchedAt
        ON [weather].[WeatherRecords] (City, Country, FetchedAt DESC);
END