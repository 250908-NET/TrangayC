using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace firstAPI.Endpoints;

public static class GameEndPoints
{
    public static IEndpointRouteBuilder MapGameEndPoints(this IEndpointRouteBuilder app)
    {
        var gamePath = app.MapGroup("/game");

        // POST /game/guess-number
        // Body: { "guess": 42 } — compares against a session target (random 1..100 for app run)
        gamePath.MapPost("guess-number", ([FromBody] GuessBody body) =>
        {
            if (body is null) return Results.BadRequest(new { error = "body is required" });
            if (body.guess < 1 || body.guess > 100) return Results.BadRequest(new { error = "guess must be between 1 and 100" });

            var target = EnsureSessionTarget();

            string hint;
            if (body.guess == target)
            {
                hint = "correct";
            }
            else if (body.guess < target)
            {
                hint = "higher";
            }
            else
            {
                hint = "lower";
            }

            return Results.Ok(new { guess = body.guess, hint });
        });

        // POST /game/guess-number/reset -> regenerates the session target
        gamePath.MapPost("guess-number/reset", () =>
        {
            randomNumber = rng.Next(1, 101);
            return Results.Ok(new { message = "secret number was reset" });
        });

        // GET /game/rock-paper-scissors/{choice}
        gamePath.MapGet("rock-paper-scissors/{choice}", (string choice) =>
        {
            if (string.IsNullOrWhiteSpace(choice)) return Results.BadRequest(new { error = "choice is required: rock, paper, scissors" });
            var userChoice = choice.Trim().ToLowerInvariant();
            string[] validChoices = { "rock", "paper", "scissors" };
            if (!validChoices.Contains(userChoice))
                return Results.BadRequest(new { error = "invalid choice. use: rock, paper, scissors" });

            var computerChoice = validChoices[rng.Next(validChoices.Length)];

            string outcome = userChoice == computerChoice ? "draw"
                : (userChoice, computerChoice) switch
                {
                    ("rock", "scissors") => "win",
                    ("paper", "rock") => "win",
                    ("scissors", "paper") => "win",
                    _ => "lose"
                };

            return Results.Ok(new { userChoice, computerChoice, outcome });
        });

        // GET /game/dice/{sides}/{count}
        gamePath.MapGet("dice/{sides:int}/{count:int}", (int sides, int count) =>
        {
            if (sides < 4) return Results.BadRequest(new { error = "sides must be >= 4" });
            if (count < 1 || count > 100) return Results.BadRequest(new { error = "count must be between 1 and 100" });

            var rolls = new List<int>(count);
            for (int i = 0; i < count; i++) rolls.Add(rng.Next(1, sides + 1));
            return Results.Ok(new { sides, count, rolls, total = rolls.Sum(), average = rolls.Average() });
        });

        // GET /game/coin-flip/{count}
        gamePath.MapGet("coin-flip/{count:int}", (int count) =>
        {
            if (count < 1 || count > 100) return Results.BadRequest(new { error = "count must be between 1 and 100" });
            var flips = new List<string>(count);
            int headCounter = 0, tailCounter = 0;
            for (int i = 0; i < count; i++)
            {
                bool isHead = rng.Next(2) == 0;
                if (isHead) { headCounter++; flips.Add("heads"); } else { tailCounter++; flips.Add("tails"); }
            }
            return Results.Ok(new { count, headCounter, tailCounter, flips });
        });

        return app;
    }

    // helpers and models
    private static readonly Random rng = new();
    private static int? randomNumber;

    private static int EnsureSessionTarget()
    {
        if (randomNumber is null) randomNumber = rng.Next(1, 101);
        return randomNumber.Value;
    }

    public record GuessBody
    {
        public int guess { get; init; }
    }
}
