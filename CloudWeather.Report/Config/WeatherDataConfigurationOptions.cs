namespace CloudWeather.Report.Config
{
    public class WeatherDataConfigurationOptions
    {
        public string PrecipitationDataProtocol { get; set; }
        public string PrecipitationDataHost { get; set; }
        public string PrecipitationDataPort { get; set; }
        public string PrecipitationDataPath { get; set; }
        public string TemperatureDataProtocol { get; set; }
        public string TemperatureDataHost { get; set; }
        public string TemperatureDataPort { get; set; }
        public string TemperatureDataPath { get; set; }
    }
}