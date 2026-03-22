CREATE DATABASE [WeatherCache]
GO

USE [WeatherCache]
GO

IF NOT EXISTS (
    SELECT * FROM sys.schemas WHERE name = 'weather'
)
BEGIN
    EXEC('CREATE SCHEMA weather');
END
GO