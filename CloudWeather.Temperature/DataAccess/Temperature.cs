namespace CloudWeather.Temperature.DataAccess
{
    public class Temperature
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal TemperatureHighF { get; set; }
        public decimal TemperatureLowF { get; set; }
        public string ZipCode { get; set; }
    }
}