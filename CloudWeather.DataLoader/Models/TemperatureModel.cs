namespace CloudWeather.DataLoader.Models;

internal class TemperatureModel
{
    public DateTime CreatedOn { get; set; }
    public decimal TemperatureHighF { get; set; }
    public decimal TemperatureLowF { get; set; }
    public string ZipCode { get; set; }

}
