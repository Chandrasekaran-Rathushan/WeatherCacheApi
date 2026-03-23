Project Idea: Weather API with Caching
======================================
It’s a weather API with caching that sits between your app and OpenWeatherMap.

When you request a city’s weather, it first checks a local SQL Server database:

	If data from the last hour exists -> returns it instantly.
	If not -> fetches fresh data, saves it, then returns it.

This makes repeated requests faster and reduces external API usage.

You can

	View all cached cities
	Get a record by ID
	Search weather by cit

Summary: it’s a REST API that caches weather data locally to save cost, improve speed, and keep recent history.


API Endpoint: http://api.openweathermap.org/data/2.5//weather?q=<string>&id=<integer>&lat=<number>&lon=<number>&zip=<string>&units=<string>&lang=<string>&Mode=<string>&appid=<string>&No Name=<string>
Response: JSON

        {
            "coord": {
                "lon": 79.8478,
                "lat": 6.9319
            },
            "weather": [
                {
                    "id": 804,
                    "main": "Clouds",
                    "description": "overcast clouds",
                    "icon": "04n"
                }
            ],
            "base": "stations",
            "main": {
                "temp": 301.5,
                "feels_like": 306.14,
                "temp_min": 301.5,
                "temp_max": 302.12,
                "pressure": 1011,
                "humidity": 80,
                "sea_level": 1011,
                "grnd_level": 1010
            },
            "visibility": 10000,
            "wind": {
                "speed": 1.71,
                "deg": 192,
                "gust": 1.6
            },
            "clouds": {
                "all": 85
            },
            "dt": 1774109127,
            "sys": {
                "type": 2,
                "id": 2103021,
                "country": "LK",
                "sunrise": 1774053869,
                "sunset": 1774097477
            },
            "timezone": 19800,
            "id": 1248991,
            "name": "Colombo",
            "cod": 200
        }

## Prerequisites
- .NET 10.0 SDK
- SQL Server (Express or any edition)
- OpenWeatherMap API Key

## Clone the repository
    git clone https://github.com/Chandrasekaran-Rathushan/WeatherCacheApi


## How to obtain an Api Key
    - Go to https://home.openweathermap.org/users/sign_u and then login to openweathermap
    - Then go to https://home.openweathermap.org/api_keys and then generate an api key
    - In the project directory, run: dotnet user-secrets init
    - Set your secret: dotnet user-secrets set "OpenWeather:ApiKey" "your_secret_value"

## Table Schema
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

## API Endpoints and usage

    1. curl 'https://localhost:7081/api/Weather/city/{city}'
      This is used to retrieve weather information for a given city. 
      The query first checks whether data from the past hour exists in the database. 
      If no recent data is found, the OpenWeather API is called, and the retrieved data is stored in the application database.

    2. curl https://localhost:7081/api/Weather
       Get all cached weather data in the table

    3. curl https://localhost:7081/api/Weather/1
       Retrieve the stored cached weather data using the Id colum

## Framework and Libraries Used
    - Microsoft.AspNetCore.App: The ASP.NET Core framework for building web apps
    - Microsoft.NETCore.App: - The core runtime & libraries for .NET applications

    - Microsoft.Data.SqlClient: The official MS SQL Server driver
    - Scalar.AspNetCore: Renders the interactive API documentation 
    - Microsoft.AspNetCore.OpenApi: A built-in package that generates an OpenAPI spec at /openapi/v1.json, which Scalar uses for its UI.
   

## How to build and run the project
    Go to the project directory and run the following commands:
        dotnet restore
        dotnet build -c Release
        dotnet run --project WeatherCacheApi.csproj

    The API will listen on `http://localhost:7081` 

     Apis can be exlpored through scalar UI: https://localhost:7081/scalar/v1
