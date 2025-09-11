using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UnitsNet;
using UnitsNet.Units;

namespace firstAPI.Endpoints;

public static class ConvertEndPoints
{
    public static IEndpointRouteBuilder MapConvertEndPoints(this IEndpointRouteBuilder app)
    {
        var convertPath = app.MapGroup("/convert");
        // Checks if unit is valid, if not return BadRequest
        // Converts into the unit enum value
        // Use the enum to convert the value
        // Return the result
        convertPath.MapGet("length/{value}/{fromUnit}/{toUnit}", (double value, string fromUnit, string toUnit) =>
        {
            if (!UnitParser.Default.TryParse<LengthUnit>(fromUnit, out var fromEnum) ||
                !UnitParser.Default.TryParse<LengthUnit>(toUnit, out var toEnum))
                return Results.BadRequest(new { error = "Invalid unit. Allowed: meters(m), feet(ft), inches(in)." });
            double result = Length.From(value, fromEnum).As(toEnum);
            return Results.Ok(new { value, from = fromEnum.ToString(), to = toEnum.ToString(), result });
        });

        convertPath.MapGet("weight/{value}/{fromUnit}/{toUnit}", (double value, string fromUnit, string toUnit) =>
        {
            if (!UnitParser.Default.TryParse<MassUnit>(fromUnit, out var fromEnum) ||
                !UnitParser.Default.TryParse<MassUnit>(toUnit, out var toEnum))
                return Results.BadRequest(new { error = "Invalid unit. Allowed: kilograms(kg), pounds(lb/lbs), ounces(oz)." });
            double result = Mass.From(value, fromEnum).As(toEnum);
            return Results.Ok(new { value, from = fromEnum.ToString(), to = toEnum.ToString(), result });
        });

        convertPath.MapGet("volume/{value}/{fromUnit}/{toUnit}", (double value, string fromUnit, string toUnit) =>
        {
            if (!UnitParser.Default.TryParse<VolumeUnit>(fromUnit, out var fromEnum) ||
                !UnitParser.Default.TryParse<VolumeUnit>(toUnit, out var toEnum))
                return Results.BadRequest(new { error = "Invalid unit. Allowed: liters(l), gallons(gal), cups(cup)." });
            double result = Volume.From(value, fromEnum).As(toEnum);
            return Results.Ok(new { value, from = fromEnum.ToString(), to = toEnum.ToString(), result });
        });

        convertPath.MapGet("list-units/{type}", (string type) =>
        {
            if (string.IsNullOrWhiteSpace(type)) return Results.BadRequest(new { error = "Type is required. Use: length, weight, volume." });

            string t = type.ToLower();
            return t switch
            {
                "length" => Results.Ok(new { type = t, units = new[] { "m", "ft", "in" } }),
                "weight" => Results.Ok(new { type = t, units = new[] { "kg", "lb", "oz" } }),
                "volume" => Results.Ok(new { type = t, units = new[] { "l", "gal", "cup" } }),
                _ => Results.BadRequest(new { error = "Unknown type. Use: length, weight, volume." })
            };
        });

        return app;
    }
}
