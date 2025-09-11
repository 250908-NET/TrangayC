using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class ColorsEndpoints
{
    private static readonly List<string> ColorsStore = new() { "red", "blue", "green", "yellow", "purple" };

    public static IEndpointRouteBuilder MapColorsEndpoints(this IEndpointRouteBuilder app)
    {
        var colorsPath = app.MapGroup("/colors");

        colorsPath.MapGet("", () => Results.Ok(ColorsStore));

        colorsPath.MapGet("random", () =>
        {
            var random = new Random();
            var color = ColorsStore[random.Next(ColorsStore.Count)];
            return Results.Ok(color);
        });

        colorsPath.MapGet("search/{letter}", (string letter) =>
        {
            if (string.IsNullOrEmpty(letter)) return Results.Ok(new List<string>());
            var result = ColorsStore
                .Where(c => c.StartsWith(letter, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return Results.Ok(result);
        });

        colorsPath.MapPost("add/{color}", (string color) =>
        {
            if (string.IsNullOrWhiteSpace(color)) return Results.BadRequest(new { error = "color must not be empty" });
            if (!ColorsStore.Any(c => string.Equals(c, color, StringComparison.OrdinalIgnoreCase)))
            {
                ColorsStore.Add(color.Trim());
            }
            return Results.Ok(ColorsStore);
        });

        return app;
    }
}
