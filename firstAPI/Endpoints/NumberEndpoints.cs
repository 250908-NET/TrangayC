using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class NumberEndpoints
{
    public static IEndpointRouteBuilder MapNumberEndpoints(this IEndpointRouteBuilder app)
    {
        var numberPath = app.MapGroup("/numbers");

        numberPath.MapGet("fizzbuzz/{count}", (int count) =>
        {
            var result = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                bool divisibleBy3 = (i % 3 == 0);
                bool divisibleBy5 = (i % 5 == 0);
                if (divisibleBy3 && divisibleBy5) result.Add("FizzBuzz");
                else if (divisibleBy3) result.Add("Fizz");
                else if (divisibleBy5) result.Add("Buzz");
                else result.Add(i.ToString());
            }
            return Results.Ok(result);
        });

        numberPath.MapGet("prime/{number}", (int number) =>
        {
            if (number <= 1) return Results.Ok(new { isPrime = false });
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0) return Results.Ok(new { isPrime = false });
            }
            return Results.Ok(new { isPrime = true });
        });

        numberPath.MapGet("fibonacci/{count}", (int count) =>
        {
            var result = new List<int>();
            int a = 0, b = 1;
            for (int i = 0; i < count; i++)
            {
                result.Add(a);
                int temp = a; a = b; b = temp + b;
            }
            return Results.Ok(result);
        });

        numberPath.MapGet("factors/{number}", (int number) =>
        {
            var result = new List<int>();
            for (int i = 1; i <= number; i++) if (number % i == 0) result.Add(i);
            return Results.Ok(result);
        });

        return app;
    }
}
