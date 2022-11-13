using System.Text.Json;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using CloudWeather.Report.Models;

namespace CloudWeather.Report.BusinessLogic
{
    public interface IWeatherReportAggregator
    {
        Task<WeatherReport> GetWeatherReport(string zipCode, int days);
    }

    public class WeatherReportAggregator : IWeatherReportAggregator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<WeatherReportAggregator> _logger;
        private readonly WeatherDataConfiguration _weatherDataConfiguration;
        private readonly WeatherReportDbContext _weatherReportDbContext;

        private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public WeatherReportAggregator
        (
            IHttpClientFactory http,
            ILogger<WeatherReportAggregator> logger,
            WeatherDataConfiguration weatherDataConfiguration,
            WeatherReportDbContext weatherReportDbContext
        )
        {
            _http = http;
            _logger = logger;
            _weatherDataConfiguration = weatherDataConfiguration;
            _weatherReportDbContext = weatherReportDbContext;
        }

        public async Task<WeatherReport> GetWeatherReport(string zipCode, int days)
        {
            var httpClient = _http.CreateClient();

            var precipData = await GetPrecipitations(httpClient, zipCode, days);

            var totalSnow = GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);

            _logger.LogInformation(
                $"zip: {zipCode}, days: {days}, totalSnow: {totalSnow}, totalRain: {totalRain}"
            );

            //var avgTemp = GetAverageTemperature(tempData);

            var tempData = await GetTemperatures(httpClient, zipCode, days);
            var averageHighTemp = tempData.Average(t => t.TemperatureHighF);
            var averageLowTemp = tempData.Average(t => t.TemperatureLowF);

            _logger.LogInformation(
                $"zip: {zipCode}, days: {days}, averageHighTemp: {averageHighTemp}, averageLowTemp: {averageLowTemp}"
            );

            var weatherReport = new WeatherReport
            {
                AverageHighF = Math.Round(averageHighTemp, 1),
                AverageLowF = Math.Round(averageLowTemp, 1),
                RainfallTotalInches = totalRain,
                SnowTotalInches = totalSnow,
                ZipCode = zipCode,
                CreatedOn = DateTime.UtcNow,
            };

            //_weatherReportDbContext.Add(weeklyWeatherReport);

            _weatherReportDbContext.WeatherReports.Add(weatherReport);
            await _weatherReportDbContext.SaveChangesAsync();

            return weatherReport;
        }

        private static decimal GetTotalSnow(IEnumerable<PrecipitationModel> precipData)
        {
            var totalSnow = precipData.Where(p => p.WeatherType == "snow").Sum(p => p.AmountInches);
            return Math.Round(totalSnow, 1);
        }

        private static decimal GetTotalRain(IEnumerable<PrecipitationModel> precipData)
        {
            var totalRain = precipData.Where(p => p.WeatherType == "rain").Sum(p => p.AmountInches);
            return Math.Round(totalRain, 1);
        }

        private async Task<List<TemperatureModel>> GetTemperatures(HttpClient httpClient, string zipCode, int days)
        {
            var endpoint = BuildTemperatureServiceEndpoint(zipCode, days);
            var temperatureRecords = await GetTemperatureRecords(httpClient, endpoint);
            return temperatureRecords;
        }

        private async Task<List<TemperatureModel>> GetTemperatureRecords(HttpClient httpClient, string endpoint)
        {
            var temperatureData = await httpClient.GetAsync(endpoint);
            var temperatureModel = await temperatureData.Content.ReadFromJsonAsync<List<TemperatureModel>>(_jsonSerializerOptions);
            return temperatureModel ?? new List<TemperatureModel>();
        }

        private async Task<List<PrecipitationModel>> GetPrecipitations(HttpClient httpClient, string zipCode, int days)
        {
            var endpoint = BuildPrecipitationServiceEndpoint(zipCode, days);
            var precipitationRecords = await GetPrecipitationRecords(httpClient, endpoint);
            return precipitationRecords;
        }


        private async Task<List<PrecipitationModel>> GetPrecipitationRecords(HttpClient httpClient, string endpoint)
        {
            var precipitationData = await httpClient.GetAsync(endpoint);
            var precipitationModel = await precipitationData.Content.ReadFromJsonAsync<List<PrecipitationModel>>(_jsonSerializerOptions);
            return precipitationModel ?? new List<PrecipitationModel>();
        }

        private string BuildTemperatureServiceEndpoint(string zipCode, int days)
        {
            return $"{_weatherDataConfiguration.TemperatureDataProtocol}://{_weatherDataConfiguration.TemperatureDataHost}:{_weatherDataConfiguration.TemperatureDataPort}/{_weatherDataConfiguration.TemperatureDataPath}?zipCode={zipCode}&days={days}";
        }

        private string BuildPrecipitationServiceEndpoint(string zipCode, int days)
        {
            return $"{_weatherDataConfiguration.PrecipitationDataProtocol}://{_weatherDataConfiguration.PrecipitationDataHost}:{_weatherDataConfiguration.PrecipitationDataPort}/{_weatherDataConfiguration.PrecipitationDataPath}?zipCode={zipCode}&days={days}";
        }

    }
}