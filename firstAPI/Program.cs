using System.Security.Cryptography;
using firstAPI.Endpoints;
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
// EndPoints in other folder
app.MapCalculatorEndpoints();
app.MapTextEndpoints();
app.MapNumberEndpoints();
app.MapDateEndpoints();
app.MapColorsEndpoints();
app.MapTempEndpoints();
app.MapPasswordEndpoints();
app.MapValidateEndpoints();
app.MapConvertEndPoints();
app.MapForecastEndPoints();
app.MapGameEndPoints();

app.Run();