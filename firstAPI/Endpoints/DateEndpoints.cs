using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class DateEndpoints
{
    public static IEndpointRouteBuilder MapDateEndpoints(this IEndpointRouteBuilder app)
    {
        var datePath = app.MapGroup("/date");

        datePath.MapGet("today", () =>
        {
            var today = DateTime.Now;
            return Results.Ok(new
            {
                iso = today.ToString("yyyy-MM-dd"),
                us = today.ToString("MM/dd/yyyy"),
                eu = today.ToString("dd/MM/yyyy")
            });
        });

        datePath.MapGet("age/{birthYear}", (int birthYear) =>
        {
            var today = DateTime.Now;
            var age = today.Year - birthYear;
            return Results.Ok(new { age });
        });

        datePath.MapGet("daysbetween/{date1}/{date2}", (DateTime date1, DateTime date2) =>
        {
            var daysBetween = date2 - date1;
            return Results.Ok(new { daysBetween = daysBetween.Days });
        });

        datePath.MapGet("weekday/{date}", (DateTime date) =>
        {
            var weekday = date.DayOfWeek;
            return Results.Ok(new { weekday });
        });

        return app;
    }
}
