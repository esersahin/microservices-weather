using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/observation/{zip}", (string zip,[FromQuery] int? days) =>
{
    return Results.Ok(zip);
    // var observation = new Observation();
    // observation.Zip = zip;
    // observation.Temperature = 70;
    // observation.Precipitation = 0.0;
    // return observation;
});

app.Run();
