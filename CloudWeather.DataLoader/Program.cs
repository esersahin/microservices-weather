using System.Net.Http.Json;
using CloudWeather.DataLoader.Models;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var servicesConfig = config.GetSection("Services");

var tempServiceConfig = servicesConfig.GetSection("TemperatureService");
var tempServiceHost = tempServiceConfig["Host"];
var tempServicePort = tempServiceConfig["Port"];

var precipServiceConfig = servicesConfig.GetSection("PrecipitationService");
var precipServiceHost = precipServiceConfig["Host"];
var precipServicePort = precipServiceConfig["Port"];

var zipCodes = new List<string> {
    "73026",
    "68104",
    "04401",
    "32808",
    "19717"
};

Console.WriteLine("Starting Weather Report Aggregator");

var temperatureHttpClient = new HttpClient();
temperatureHttpClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var precipitationHttpClient = new HttpClient();
precipitationHttpClient.BaseAddress = new Uri($"http://{precipServiceHost}:{precipServicePort}");

foreach (var zip in zipCodes)
{
    Console.WriteLine($"Processing zip code: {zip}");
    var from = DateTime.Now.AddDays(-2);
    var thru = DateTime.Now;

    for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
    {
        var temps = PostTemperature(zip, day, temperatureHttpClient);
        PostPrecipitation(temps[0], zip, day, precipitationHttpClient);
    }
}

void PostPrecipitation(int lowTemp, string zip, DateTime day, HttpClient precipitationHttpClient)
{
    var rand = new Random();
    var isPrecip = rand.Next(2) < 1;

    PrecipitationModel precipitation;

    if (isPrecip)
    {
        var precipInches = rand.Next(1, 16);
        if (lowTemp < 32)
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "Snow",
                ZipCode = zip,
                CreatedOn = day
            };
        }
        else
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "Rain",
                ZipCode = zip,
                CreatedOn = day
            };
        }
    }
    else
    {
        precipitation = new PrecipitationModel
        {
            AmountInches = 0,
            WeatherType = "None",
            ZipCode = zip,
            CreatedOn = day
        };
    }

    var response = precipitationHttpClient.PostAsJsonAsync("observation", precipitation).Result;

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted precipitation Date : {day:d} " +
                          $"Zip:{zip} " +
                          $"Type: {precipitation.WeatherType} " +
                          $"Amount (in.): {precipitation.AmountInches}");
    }
    else
    {
        Console.WriteLine($"Failed to post precipitation for {zip} on {day}");
    }

    //response.EnsureSuccessStatusCode();
    //return response.Content.ReadFromJsonAsync<PrecipitationModel>().Result;
}

List<int> PostTemperature(string zip, DateTime day, HttpClient temperatureHttpClient)
{
    var rand = new Random();
    var lowTemp = rand.Next(0, 100);
    var highTemp = rand.Next(lowTemp, 100);
    var hilo = new List<int> { lowTemp, highTemp };
    hilo.Sort();

    var temp = new TemperatureModel
    {
        TemperatureLowF = hilo[0],
        TemperatureHighF = hilo[1],
        CreatedOn = day,
        ZipCode = zip
    };

    var response = temperatureHttpClient.PostAsJsonAsync("observation", temp).Result;

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted temperature Date : {day:d} " +
                          $"Zip:{zip} " +
                          $"Low (F): {temp.TemperatureLowF} " +
                          $"High (F): {temp.TemperatureHighF}");
    }
    else
    {
        Console.WriteLine($"Failed to post temperature for {zip} on {day}");
    }

    return hilo;
    //response.EnsureSuccessStatusCode();
    //return response.Content.ReadFromJsonAsync<TemperatureModel>().Result;
}


// foreach(var zip in zipCodes)
// {
//     var precipData = await GetPrecipitations(precipitationHttoClient, zip, 5);
//     var totalSnow = GetTotalSnow(precipData);
//     var totalRain = GetTotalRain(precipData);

//     Console.WriteLine($"zip: {zip}, totalSnow: {totalSnow}, totalRain: {totalRain}");

//     var tempData = await GetTemperatures(temperatureHttoClient, zip, 5);
//     var averageHighTemp = tempData.Average(t => t.TemperatureHighF);
//     var averageLowTemp = tempData.Average(t => t.TemperatureLowF);

//     Console.WriteLine($"zip: {zip}, averageHighTemp: {averageHighTemp}, averageLowTemp: {averageLowTemp}");
// }

// Console.WriteLine("Finished Weather Report Aggregator");

// async Task<List<PrecipitationModel>> GetPrecipitations(HttpClient httpClient, string zipCode, int days)
// {
//     var response = await httpClient.GetAsync($"precipitation/{zipCode}/{days}");
//     response.EnsureSuccessStatusCode();

//     var precipData = await response.Content.ReadFromJsonAsync<List<PrecipitationModel>>();
//     return precipData;
// }

// async Task<List<TemperatureModel>> GetTemperatures(HttpClient httpClient, string zipCode, int days)
// {
//     var response = await httpClient.GetAsync($"temperature/{zipCode}/{days}");
//     response.EnsureSuccessStatusCode();

//     var tempData = await response.Content.ReadFromJsonAsync<List<TemperatureModel>>();
//     return tempData;
// }

// decimal GetTotalSnow(IEnumerable<PrecipitationModel> precipData)
// {
//     var totalSnow = precipData.Where(p => p.WeatherType == "snow").Sum(p => p.AmountInches);
//     return Math.Round(totalSnow, 1);
// }

// decimal GetTotalRain(IEnumerable<PrecipitationModel> precipData)
// {
//     var totalRain = precipData.Where(p => p.WeatherType == "rain").Sum(p => p.AmountInches);
//     return Math.Round(totalRain, 1);
// }




// var weatherReportAggregator = new WeatherReportAggregator(
//     temperatureHttoClient,
//     precipitationHttoClient,
//     new Logger<WeatherReportAggregator>(new LoggerFactory())
// );

// foreach (var zipCode in zipCodes)
// {
//     var weatherReport = weatherReportAggregator.GetWeatherReport(zipCode, 5).Result;
//     Console.WriteLine(weatherReport);
// }

// Console.WriteLine("Finished Weather Report Aggregator");
