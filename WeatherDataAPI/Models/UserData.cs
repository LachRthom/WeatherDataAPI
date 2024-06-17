using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WeatherDataAPI.Models
{
    public class UserData
    {
        [BsonId]
        public ObjectId _id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public DateTime LastLoginDate { get; set; }

        public string ApiKey { get; set; }  
    }

}