using API.MessagePublisher;
using API.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.API;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAPICors();
builder.Services.AddAPIOpenTelemetry();
builder.Services.AddAPIMassTransit();
builder.Services.AddDbContext<JourneyDbContext>(options => options.UseNpgsql("server=postgres;port=5432;user id=user;password=pass;database=journey_database"));
builder.Services.AddTransient<JourneyStartPublisher>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapPost("/journey/start", async ([FromBody] JourneyRequest request, [FromServices] JourneyDbContext dbContext, [FromServices] JourneyStartPublisher publisher) =>
{
    await dbContext.Database.EnsureCreatedAsync();

    var userId = Guid.NewGuid();
    var activity = Activity.Current;
    activity?.SetTag("api.user.id", userId);
    activity?.SetTag("api.user.username", request.Username);

    var entry = new JourneyEntry(userId, request.Username, DateTime.UtcNow);
    await dbContext.JourneyEntries.AddAsync(entry);
    await dbContext.SaveChangesAsync();

    await publisher.PublishAsync(entry, CancellationToken.None);

    return Results.Ok("api.user.id=" + entry.Id.ToString());
})
.WithName("Start Journey")
.WithOpenApi();

app.Run();
