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