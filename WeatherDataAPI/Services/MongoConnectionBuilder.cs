using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WeatherDataAPI.Settings;

namespace WeatherDataAPI.Services
{

    // Creates the MongoDB connections when required
    public class MongoConnectionBuilder
    {
        // Grabs the connection string and database name from the dependency injection system
        private readonly IOptions<MongoConnectionSettings> _options;

        // Gives us a local version of conn string and DB name to use within this class
        public MongoConnectionBuilder(IOptions<MongoConnectionSettings> options) 
        {  
            _options = options; 
        }


        // Retrieves and returns a reference to the MongoDB database based on the provided connection settings
        public IMongoDatabase GetDatabase()
        {
            // Instantiates a new MongoClient using the connection string retrieved from the injected options
            var client = new MongoClient(_options.Value.ConnectionString);

            // Retrieves and returns the MongoDB database using the database name retrieved from the injected options
            return client.GetDatabase(_options.Value.DatabaseName);
        }



    }
}
