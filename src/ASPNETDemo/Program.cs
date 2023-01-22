using ASPNETDemo;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenTelemetryTracingDemo();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var activitySource = OpenTelemetryConfiguration.GetActivitySource();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    using var activity = activitySource.StartActivity("weatherforecast activity", ActivityKind.Internal);

    var tagCollection = new List<KeyValuePair<string, object>>()
    {
        KeyValuePair.Create<string, object>("Item 1", "0001"),
        KeyValuePair.Create<string, object>("Item 2", "0002"),
        KeyValuePair.Create<string, object>("Item 3", "0003")
    };

    activity?.AddEvent(new ActivityEvent("Requested weather report", tags: new ActivityTagsCollection(tagCollection!)));

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    activity?.SetTag("Item 3", "0003");

    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}