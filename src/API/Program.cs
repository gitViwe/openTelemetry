using API.MessagePublisher;
using API.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.API;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
            .AddAPICors()
            .AddAPIMassTransit()
            .AddAPISeqLogging()
            .AddDbContext<JourneyDbContext>(options => options.UseNpgsql("server=postgres;port=5432;user id=user;password=pass;database=journey_database"))
            .AddTransient<JourneyStartPublisher>()
            .AddAPIOpenTelemetry();

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

    var current = await dbContext.JourneyEntries.AddAsync(new JourneyEntry(Guid.NewGuid(), request.Username, DateTime.UtcNow));
    await dbContext.SaveChangesAsync();

    await publisher.PublishAsync(current.Entity, CancellationToken.None);

    return Results.Accepted("api.user.id=" + current.Entity.Id.ToString());
})
.WithName("Start Journey")
.WithOpenApi();

app.Run();
