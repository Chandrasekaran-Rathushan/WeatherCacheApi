using Scalar.AspNetCore;
using SharedKernel.Middlewares;
using WeatherCacheApi.Data;
using WeatherCacheApi.Repositories.OpenWeatherService;
using WeatherCacheApi.Repositories.Weather;
using WeatherCacheApi.Services.Weather;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<SqlDbContext>();

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

builder.Services.AddScoped<DatabaseInitializer>();

builder.Services.AddHttpClient<IOpenWeatherService, OpenWeatherService>();

builder.Services.AddScoped<IWeatherService, WeatherService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// DB init on project startup
using (var scope = app.Services.CreateScope())
{
    var dbInit = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await dbInit.InitializeAsync();
}


app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Weather Cache API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
