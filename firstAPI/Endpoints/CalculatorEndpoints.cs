using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class CalculatorEndpoints
{
    public static IEndpointRouteBuilder MapCalculatorEndpoints(this IEndpointRouteBuilder app)
    {
        var calculatorPath = app.MapGroup("/calculator");

        calculatorPath.MapGet("/add/{a}/{b}", (int a, int b) =>
        {
            var result = new { operation = "add", input1 = a, input2 = b, result = a + b };
            return Results.Ok(result);
        });

        calculatorPath.MapGet("/subtract/{a}/{b}", (int a, int b) =>
        {
            var result = new { operation = "subtract", input1 = a, input2 = b, result = a - b };
            return Results.Ok(result);
        });

        calculatorPath.MapGet("/multiply/{a}/{b}", (int a, int b) =>
        {
            var result = new { operation = "multiply", input1 = a, input2 = b, result = a * b };
            return Results.Ok(result);
        });

        calculatorPath.MapGet("/divide/{a}/{b}", (double a, double b) =>
        {
            if (b == 0)
            {
                var error = new { error = "Cannot divide by zero" };
                return Results.BadRequest(error);
            }
            var result = new { operation = "divide", input1 = a, input2 = b, result = a / b };
            return Results.Ok(result);
        });

        return app;
    }
}
