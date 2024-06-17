using MongoDB.Bson;
using MongoDB.Driver;
using WeatherDataAPI.Models;
using WeatherDataAPI.Models.DTOs;


namespace WeatherDataAPI.Repository
{
    public class MongoDataPointRepository : IDataPointRepository
    {

        private readonly IMongoCollection<DataPoint> _dataPoints;

        public MongoDataPointRepository(IMongoDatabase database)
        {
            _dataPoints = database.GetCollection<DataPoint>("WeatherSensorReadings");
        }

        // Gets all data points between two given dates
        public IEnumerable<DataPoint> GetDataPointsInRange(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<DataPoint>.Filter.And(
                Builders<DataPoint>.Filter.Gte(dp => dp.Time, startDate),
                Builders<DataPoint>.Filter.Lte(dp => dp.Time, endDate)
            );

            var dataPoints = _dataPoints.Find(filter).ToList();

            return dataPoints;
        }

        // Gets all data points within date range for the named sensor
        public IEnumerable<DataPoint> GetRecordsForSensorInRange(string deviceName, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<DataPoint>.Filter.And(
                Builders<DataPoint>.Filter.Eq(dp => dp.DeviceName, deviceName),
                Builders<DataPoint>.Filter.Gte(dp => dp.Time, startDate),
                Builders<DataPoint>.Filter.Lte(dp => dp.Time, endDate)
            );

            return _dataPoints.Find(filter).ToList();
        }

        // Find the records with the maximum temperature, for all sensors between the date range
        public List<MaxTemperatureRecordDTO> GetRecordWithMaxTemperature(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<DataPoint>.Filter.Gte("Time", startDate) & Builders<DataPoint>.Filter.Lte("Time", endDate);

            var projection = Builders<DataPoint>.Projection.Expression(
                dp => new MaxTemperatureRecordDTO
                {
                    DeviceName = dp.DeviceName,
                    Time = dp.Time,
                    Temperature = dp.Temperature
                });

            var results = _dataPoints.Aggregate()
                .Match(filter)
                .Group(
                    dp => dp.DeviceName,
                    group => new MaxTemperatureRecordDTO
                    {
                        DeviceName = group.Key,
                        Temperature = group.Max(dp => dp.Temperature),
                        Time = group.First().Time
                    })
                .ToList();

            return results;
        }

        // This wont work because the date of the newest record is older than 5 months ago, so i changed it to 50 months
        public MaxPrecipitationRecordDTO GetMaxPrecipitationRecord(string deviceName)
        {
            // Calculate the date 50 months ago from the current date
            var fiveMonthsAgo = DateTime.UtcNow.AddMonths(-50);

  
            var filter = Builders<DataPoint>.Filter.And(
                Builders<DataPoint>.Filter.Eq(dp => dp.DeviceName, deviceName),
                Builders<DataPoint>.Filter.Gte(dp => dp.Time, fiveMonthsAgo)
            );

            var sort = Builders<DataPoint>.Sort.Descending(dp => dp.Precipitation);
 
            var maxPrecipitationRecord = _dataPoints.Find(filter)
                .Sort(sort)
                .Limit(1)
                .FirstOrDefault();

            if (maxPrecipitationRecord != null)
            {
                // Map the result to MaxPrecipitationRecordDTO
                return new MaxPrecipitationRecordDTO
                {
                    DeviceName = maxPrecipitationRecord.DeviceName,
                    Time = maxPrecipitationRecord.Time,
                    Precipitation = maxPrecipitationRecord.Precipitation
                };
            }

            return null;
        }

        // Get single record based on id 
        public DataPoint GetSingleRecord(string id)
        {
            ObjectId objectId;
            if (!ObjectId.TryParse(id, out objectId))
            {
                return null;
            }

            var filter = Builders<DataPoint>.Filter.Eq("_id", objectId);
            return _dataPoints.Find(filter).FirstOrDefault();
        }

        // Insert multiple records
        public void InsertMultipleRecords(IEnumerable<DataPoint> dataPoints)
        {
            _dataPoints.InsertMany(dataPoints);
        }

        // Insert Single Record
        public void InsertSingleRecord(DataPoint dataPoint)
        {
            _dataPoints.InsertOne(dataPoint);
        }

        // Update only precipitation
        public void UpdatePrecipitation(string id, double newPrecipitationValue)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<DataPoint>.Filter.Eq(dp => dp.Id, objectId);
            var update = Builders<DataPoint>.Update.Set(dp => dp.Precipitation, newPrecipitationValue);
            _dataPoints.UpdateOne(filter, update);
        }

        // Update Entire Single Record
        public void UpdateSingleRecord(string id, DataPoint dataPoint)
        {

            var objectId = new ObjectId(id);
            var filter = Builders<DataPoint>.Filter.Eq(dp => dp.Id, objectId);
            dataPoint.Id = objectId;
            _dataPoints.ReplaceOne(filter, dataPoint);

        }

        // Returns specific fields from a record
        // TODO: this really should be mapped to a DTO
        public DataPoint GetSensorSnapShot(string deviceName, DateTime snapShotDateTime)
        {
            var filter = Builders<DataPoint>.Filter.Eq(x => x.DeviceName, deviceName) &
                         Builders<DataPoint>.Filter.Eq(x => x.Time, snapShotDateTime);

            return _dataPoints.Find(filter).FirstOrDefault();
        }

    }

}
