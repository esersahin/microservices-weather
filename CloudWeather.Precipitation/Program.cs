using CloudWeather.Precipitation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PrecipitationDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip, [FromQuery] int? days, PrecipitationDbContext db) =>
{
    //return Results.Ok(zip);

    if (days is null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide a days query parameter between 1 and 30");
    }

    var startData = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await db.Precipitations.Where(p => p.ZipCode == zip && p.CreatedOn >= startData)
        .OrderByDescending(p => p.CreatedOn)
        .ToListAsync();

    return Results.Ok(results);
});

app.MapPost("/observation", async (Precipitation observation, PrecipitationDbContext db) =>
{
    observation.Id = Guid.NewGuid();
    //observation.CreatedOn = observation.CreatedOn.ToUniversalTime();
    observation.CreatedOn = DateTime.UtcNow;

    db.Precipitations.Add(observation);
    await db.SaveChangesAsync();

    return Results.Created($"/observation/{observation.ZipCode}", observation);
});

app.Run();
