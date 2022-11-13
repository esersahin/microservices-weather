using CloudWeather.Temperature.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TemperatureDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

var app = builder.Build();

app.MapGet("/observation/{zip}", async (string zip, [FromQuery] int? days, TemperatureDbContext db) =>
{
    //return Results.Ok(zip);

    if (days is null || days < 1 || days > 30)
    {
        return Results.BadRequest("Please provide a days query parameter between 1 and 30");
    }

    var startData = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await db.Temperatures.Where(p => p.ZipCode == zip && p.CreatedOn >= startData)
        .OrderByDescending(p => p.CreatedOn)
        .ToListAsync();

    return Results.Ok(results);

});

app.MapPost("/observation", async (Temperature observation, TemperatureDbContext db) =>
{
    observation.Id = Guid.NewGuid();
    //observation.CreatedOn = observation.CreatedOn.ToUniversalTime();
    observation.CreatedOn = DateTime.UtcNow;

    db.Temperatures.Add(observation);
    await db.SaveChangesAsync();

    return Results.Created($"/observation/{observation.ZipCode}", observation);
});

app.Run();