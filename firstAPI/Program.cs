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

//#Challenge 1: Basic Calculator
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

//#Challenge 2: String Manipulator
var textPath = app.MapGroup("/text");
textPath.MapGet("reverse/{text}", (string text) =>
{
    char[] charArray = text.ToCharArray();
    Array.Reverse(charArray);
    string result = new string(charArray);
    return Results.Ok(new { result = result });
});
 
textPath.MapGet("uppercase/{text}", (string text) =>
{
    string result = text.ToUpper();
    return Results.Ok(new { result = result });
});
 
textPath.MapGet("lowercase/{text}", (string text) =>
{
    string result = text.ToLower();
    return Results.Ok(new { result = result });
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
    if (string.IsNullOrWhiteSpace(text))
    {
        return Results.Ok(new { isPalindrome = true });
    }

    int l = 0, r = text.Length - 1;
    while (l < r)
    {
        while (l < r && !char.IsLetter(text[l]))
        {
            l++;
        }
        while (r > l && !char.IsLetter(text[r]))
        {
            r--;
        }
        if (char.ToLower(text[l]) != char.ToLower(text[r]))
        {
            return Results.Ok(new { isPalindrome = false });
        }
        l++;
        r--;
    }

    return Results.Ok(new { isPalindrome = true });
});

//#Challenge 3: Number Games
var numberPath = app.MapGroup("/numbers");
numberPath.MapGet("fizzbuzz/{count}", (int count) =>
{
    var result = new List<string>();
    for (int i = 1; i <= count; i++)
    {
        bool divisibleBy3 = (i % 3 == 0);
        bool divisibleBy5 = (i % 5 == 0);

        if (divisibleBy3 && divisibleBy5)
        {
            result.Add("FizzBuzz");
        }
        else if (divisibleBy3)
        {
            result.Add("Fizz");
        }
        else if (divisibleBy5)
        {
            result.Add("Buzz");
        }
        else
        {
            result.Add(i.ToString());
        }
    }
    return Results.Ok(result);
});
numberPath.MapGet("prime/{number}", (int number) =>
{   
    if (number <= 1)
    {
        return Results.Ok(new { isPrime = false });
    }
    for (int i = 2; i <= Math.Sqrt(number); i++)
    {
        if (number % i == 0)
        {
            return Results.Ok(new { isPrime = false });
        }
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
        int temp = a;
        a = b;
        b = temp + b;
    }
    return Results.Ok(result);
});

numberPath.MapGet("factors/{number}", (int number) =>
{
    var result = new List<int>();
    for (int i = 1; i <= number; i++)
    {
        if (number % i == 0)
        {
            result.Add(i);
        }
    }
    return Results.Ok(result);
});

//#Challenge 4: Date and Time Fun 
var datePath = app.MapGroup("/date");
datePath.MapGet("today", () =>
{
    var today = DateTime.Now;
    return Results.Ok(new { today = today.ToString("yyyy-MM-dd") });
});

datePath.MapGet("age/{birthYear}", (int birthYear) =>
{
    var today = DateTime.Now;
    var age = today.Year - birthYear;
    return Results.Ok(new { age = age });
});

datePath.MapGet("daysbetween/{date1}/{date2}", (DateTime date1, DateTime date2) =>
{
    var daysBetween = date2 - date1;
    return Results.Ok(new { daysBetween = daysBetween.Days });
});

datePath.MapGet("weekday/{date}", (DateTime date) =>
{
    var weekday = date.DayOfWeek;
    return Results.Ok(new { weekday = weekday });
});

//#Challenge 5: Simple Collections
var colorsPath = app.MapGroup("/colors");
colorsPath.MapGet("", () =>
{
    var colors = new List<string> { "red", "blue", "green", "yellow", "purple" };
    return Results.Ok(colors);
});

colorsPath.MapGet("random", () =>
{
    var colors = new List<string> { "red", "blue", "green", "yellow", "purple" };
    var random = new Random();
    var color = colors[random.Next(colors.Count)];
    return Results.Ok(color);
});

colorsPath.MapGet("search/{letter}", (string letter) =>
{
    var colors = new List<string> { "red", "blue", "green", "yellow", "purple" };
    var result = colors.Where(color => color.StartsWith(letter)).ToList();
    return Results.Ok(result);
});

colorsPath.MapPost("add/{color}", (string color) =>
{
    var colors = new List<string> { "red", "blue", "green", "yellow", "purple" };
    colors.Add(color);
    return Results.Ok(colors);
});

//#Challenge 6: Temperature Converter
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

tempPath.MapGet("compare/{temp1}/{unit1}/{temp2}/{unit2}", (double temp1, string unit1, double temp2, string unit2) =>
{
    var result = new { temp1 = temp1, unit1 = unit1, temp2 = temp2, unit2 = unit2, difference = Math.Abs(temp1 - temp2) };
    return Results.Ok(result);
});

app.Run();