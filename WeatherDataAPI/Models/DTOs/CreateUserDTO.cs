using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WeatherDataAPI.Models.DTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }
    }
}
  