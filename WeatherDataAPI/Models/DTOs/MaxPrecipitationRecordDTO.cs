using MongoDB.Bson.Serialization.Attributes;

namespace WeatherDataAPI.Models.DTOs
{
    public class MaxPrecipitationRecordDTO
    {
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }
        public DateTime Time { get; set; }
        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }
    }
}