﻿using MongoDB.Bson.Serialization.Attributes;

namespace WeatherDataAPI.Models.DTOs
{
    public class DataPointDTO
    {
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }

        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }

        public DateTime Time { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [BsonElement("Temperature (°C)")]
        public double Temperature { get; set; }

        [BsonElement("Atmospheric Pressure (kPa)")]
        public double AtmosphericPressure { get; set; }

        [BsonElement("Max Wind Speed (m/s)")]
        public double MaxWindSpeed { get; set; }

        [BsonElement("Solar Radiation (W/m2)")]
        public double SolarRadiation { get; set; }

        [BsonElement("Vapor Pressure (kPa)")]
        public double VaporPressure { get; set; }

        [BsonElement("Humidity (%)")]
        public double Humidity { get; set; }

        [BsonElement("Wind Direction (°)")]
        public double WindDirection { get; set; }
    }
}
