var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
}) 
.WithName("GetWeatherForecast");

//#Challenge 1: Basic Calculator
//Return results as JSON: {"operation": <string>, "result": <int>} with HTTP 200 response 
//Return error if input is wrong
var calculatorPath = app.MapGroup("/calculator");
calculatorPath.MapGet("/add/{a}/{b}", (int a, int b) =>
{
    var result = new { operation = "add", result = a + b };
    return Results.Ok(result); 
});
calculatorPath.MapGet("/subtract/{a}/{b}", (int a, int b) => 
{
    var result = new { operation = "subtract", result = a - b };
    return Results.Ok(result); 
});
calculatorPath.MapGet("/multiply/{a}/{b}", (int a, int b) => 
{
    var result = new { operation = "multiply", result = a * b };
    return Results.Ok(result); 
});
calculatorPath.MapGet("/divide/{a}/{b}", (int? a, int? b) =>
{
    if (b == 0)
    {
        var error = new { error = "Cannot divide by zero" };
        return Results.BadRequest(error);
    }
    var result = new { operation = "divide", result = a / b };
    return Results.Ok(result); 
});

//#Challenge 2: String Manipulator
/*
Create /text/reverse/{text} - returns reversed string
Add /text/uppercase/{text} and /text/lowercase/{text}
Create /text/count/{text} - returns character count, word count, vowel count
Add /text/palindrome/{text} - checks if text is a palindrome
*/
var textPath = app.MapGroup("/text");
textPath.MapGet("reverse/{text}", (string text) =>
{
    char[] charArray = text.ToCharArray();
    Array.Reverse(charArray);
    string result = new string(charArray);
    return result;
});
textPath.MapGet("uppercase/{text}", (string text) =>
{
    string result = text.ToUpper();
    return result;
});
textPath.MapGet("lowercase/{text}", (string text) =>
{
    string result = text.ToLower();
    return result;
});
textPath.MapGet("count/{text}", (string text) =>
{
    int textLength = text.Length;
    int whiteSpace = text.Count(element => char.IsWhiteSpace(element));
    int characterCount = textLength - whiteSpace;

    string[] words = text.Split();
    int wordCount = words.Length;

    string vowels = "aeiouAEIOU";
    var vowelCount = text.Count(element => vowels.Contains(element));

    var result = new { characterCount = characterCount, wordCount = wordCount, vowelCount = vowelCount };
    return Results.Ok(result);
});
textPath.MapGet("palindrome/{text}", (string text) =>
    {
        int left = 0;
        int right = text.Length - 1;
        var result = "true";
        while (left < right)
        {
            if (text[left] != text[right])
            {
                return "False";
            }
        }
        return result;
    }
);





app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
