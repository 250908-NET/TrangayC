using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace firstAPI.Endpoints;

public static class ForecastEndPoints
{
    public static IEndpointRouteBuilder MapForecastEndPoints(this IEndpointRouteBuilder app)
    {
        var forecastPath = app.MapGroup("/forecast");

        // GET /forecast -> list all forecasts (sorted by date)
        forecastPath.MapGet("", () =>
        {
            var items = allWeatherForeCast
                .OrderBy(f => f.Date)
                .ToList();
            return Results.Ok(items);
        });

        // GET /forecast/{date} -> fetch a single forecast by date
        forecastPath.MapGet("{date}", (DateOnly date) =>
        {
            var item = allWeatherForeCast.FirstOrDefault(f => f.Date == date);
            if (item is null)
            {
                return Results.NotFound(new { error = "forecast not found for date" });
            }
            return Results.Ok(item);
        });

        // POST /forecast -> create or update a forecast from JSON body
        // Body example: { "date": "2025-09-12", "temperature": 21, "summary": "Sunny" }
        forecastPath.MapPost("", ([FromBody] WeatherForecast forecast) =>
        {
            if (forecast is null) return Results.BadRequest(new { error = "body is required" });
            if (forecast.Date == default) return Results.BadRequest(new { error = "date is required (yyyy-MM-dd)" });

            var duplicateIndex = allWeatherForeCast.FindIndex(f => f.Date == forecast.Date);
            var isNewForecast = duplicateIndex < 0;
            if (isNewForecast)
            {
                allWeatherForeCast.Add(forecast);
                return Results.Created($"/forecast/{forecast.Date:yyyy-MM-dd}", forecast);
            }
            else
            {
                allWeatherForeCast[duplicateIndex] = forecast;
            }
            return Results.Ok(forecast);
        });

        // DELETE /forecast/{date} -> remove by date
        forecastPath.MapDelete("{date}", (DateOnly date) =>
        {
            bool removed = allWeatherForeCast.RemoveAll(f => f.Date == date) > 0;
            if (!removed) return Results.NotFound(new { error = "forecast not found for date" });
            return Results.NoContent();
        });

        return app;
    }

    // In-memory list of WeatherForecast
    private static readonly List<WeatherForecast> allWeatherForeCast = new();

    public record WeatherForecast
    {
        public DateOnly Date { get; init; }
        public int Temperature { get; init; }
        public string? Summary { get; init; }
    }
}
