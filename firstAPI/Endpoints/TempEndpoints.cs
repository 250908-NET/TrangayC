using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class TempEndpoints
{
    public static IEndpointRouteBuilder MapTempEndpoints(this IEndpointRouteBuilder app)
    {
        var tempPath = app.MapGroup("/temp");

        tempPath.MapGet("celsius-to-fahrenheit/{temp}", (double temp) =>
        {
            var result = new { celsius = temp, fahrenheit = (temp * 9 / 5) + 32 };
            return Results.Ok(result);
        });

        tempPath.MapGet("fahrenheit-to-celsius/{temp}", (double temp) =>
        {
            var result = new { fahrenheit = temp, celsius = (temp - 32) * 5 / 9 };
            return Results.Ok(result);
        });

        tempPath.MapGet("kelvin-to-celsius/{temp}", (double temp) =>
        {
            var result = new { kelvin = temp, celsius = temp - 273.15 };
            return Results.Ok(result);
        });

        // Using user's latest simplified compare: absolute difference without unit conversion
        tempPath.MapGet("compare/{temp1}/{unit1}/{temp2}/{unit2}", (double temp1, string unit1, double temp2, string unit2) =>
        {
            var result = new { temp1, unit1, temp2, unit2, difference = Math.Abs(temp1 - temp2) };
            return Results.Ok(result);
        });

        return app;
    }
}
