using MongoDB.Bson.Serialization.Attributes;

namespace WeatherDataAPI.Models.DTOs
{
    public class MaxTemperatureRecordDTO
    {
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }

        public DateTime Time { get; set; }

        [BsonElement("Temperature (°C)")]
        public double Temperature { get; set; }
    }
}