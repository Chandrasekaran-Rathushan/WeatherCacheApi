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