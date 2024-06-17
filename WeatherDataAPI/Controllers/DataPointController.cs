using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using WeatherDataAPI.Repository;
using WeatherDataAPI.Models;
using WeatherDataAPI.AttributeTags;
using WeatherDataAPI.Models.DTOs;

namespace WeatherDataAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataPointController : ControllerBase
    {
        private readonly IDataPointRepository _dataPointRepository;

        public DataPointController(IDataPointRepository dataPointRepository)
        {
            _dataPointRepository = dataPointRepository;
        }

        /// <summary>
        /// Retrieves a single data point from the database based on its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the data point.</param>
        /// <returns>An ActionResult containing the data point if found, or NotFound if no data point is found with the provided ID.</returns>

        // GET: api/DataPoint/{id}
        [HttpGet("{id}")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.STUDENT, UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns the data point if found", typeof(DataPoint))]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no data point is found with the provided ID")]
        public ActionResult<DataPoint> GetSingleRecord(string id)
        {
            var dataPoint = _dataPointRepository.GetSingleRecord(id);
            if (dataPoint == null)
            {
                return NotFound();
            }
            return Ok(dataPoint);
        }


        /// <summary>
        /// Retrieves data points for a sensor within the specified date range.
        /// </summary>
        /// <param name="deviceName">The name of the sensor device.</param>
        /// <param name="startDate">The start date of the date range in the format YYYY-MM-DD.</param>
        /// <param name="endDate">The end date of the date range in the format YYYY-MM-DD.</param>
        /// <returns>An ActionResult containing the data points if found, or BadRequest if invalid start date or end date format is provided.</returns>

        // GET: api/DataPoint/{deviceName}/range
        [HttpGet("{deviceName}/range")]
        [ApiKey(UserRolesEnum.STUDENT, UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns the data points if found", typeof(IEnumerable<DataPoint>))]
        [SwaggerResponse(400, "If invalid start date or end date format, or sensor name is provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no data points are found within the date range for the named sensor")]
        public ActionResult<IEnumerable<DataPoint>> GetRecordsForSensorInRange(string deviceName, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Check if startDate and endDate are provided and in a valid format
            if (startDate == default || endDate == default || startDate >= endDate)
            {
                return BadRequest("Invalid startDate or endDate provided");
            }

            // TODO: Add input validation for device name

            // If all input validation is passed, hand over the list of datapoint objects as dataPoints
            var dataPoints = _dataPointRepository.GetRecordsForSensorInRange(deviceName, startDate, endDate);
            return Ok(dataPoints);
        }

        /// <summary>
        /// Retrieves data points within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range in the format YYYY-MM-DD.</param>
        /// <param name="endDate">The end date of the date range in the format YYYY-MM-DD.</param>
        /// <returns>An ActionResult containing the data points if found, or BadRequest if both start date and end date are not provided or if invalid start date or end date format is provided.</returns>

        // GET: api/DataPoint/range?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        [HttpGet("range")]
        [ApiKey(UserRolesEnum.STUDENT, UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns the data points if found", typeof(IEnumerable<DataPoint>))]
        [SwaggerResponse(400, "If both start date and end date are not provided or if invalid start date or end date format is provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no data points are found with the provided date range")]
        public ActionResult<IEnumerable<DataPoint>> GetDataPointsInRange([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // Check if startDate and endDate are provided
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return BadRequest("Both startDate and endDate must be provided");
            }

            // Check if startDate and endDate are in a valid format
            if (startDate == default || endDate == default || startDate >= endDate)
            {
                return BadRequest("Invalid startDate or endDate provided");
            }

            var dataPoints = _dataPointRepository.GetDataPointsInRange(startDate.Value, endDate.Value);
            return Ok(dataPoints);
        }

        /// <summary>
        /// Inserts a single data point into the database.
        /// </summary>
        /// <param name="dataPoint">The data point to be inserted.</param>
        /// <returns>An IActionResult containing the inserted data point.</returns>
        // POST: api/DataPoint/record
        [HttpPost("record")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER, UserRolesEnum.SENSOR)]
        [SwaggerResponse(201, "Returns the inserted data point", typeof(DataPoint))]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        public IActionResult InsertSingleRecord([FromBody] DataPointDTO dataPointDTO)
        {
            var dataPoint = new DataPoint
            {
                DeviceName = dataPointDTO.DeviceName,
                Precipitation = dataPointDTO.Precipitation,
                Time = dataPointDTO.Time,
                Latitude = dataPointDTO.Latitude,
                Longitude = dataPointDTO.Longitude,
                Temperature = dataPointDTO.Temperature,
                AtmosphericPressure = dataPointDTO.AtmosphericPressure,
                MaxWindSpeed = dataPointDTO.MaxWindSpeed,
                SolarRadiation = dataPointDTO.SolarRadiation,
                VaporPressure = dataPointDTO.VaporPressure,
                Humidity = dataPointDTO.Humidity,
                WindDirection = dataPointDTO.WindDirection,
                Id = ObjectId.Empty
            };
            _dataPointRepository.InsertSingleRecord(dataPoint);
            return Ok(dataPoint);
        }


        /// <summary>
        /// Inserts multiple data points into the database.
        /// </summary>
        /// <param name="dataPoints">The collection of data points to be inserted.</param>
        /// <returns>An IActionResult containing the inserted data points.</returns>
        // POST: api/DataPoint/multiple records
        [HttpPost("multiple records")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER, UserRolesEnum.SENSOR)]
        [SwaggerResponse(201, "Returns the inserted data points", typeof(IEnumerable<DataPointDTO>))]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        public IActionResult InsertMultipleRecords([FromBody] IEnumerable<DataPointDTO> dataPointDTOs)
        {
            // Map each DataPointDTO to DataPoint and insert into repository
            var dataPoints = new List<DataPoint>();
            foreach (var dataPointDto in dataPointDTOs)
            {
                var dataPoint = new DataPoint
                {
                    DeviceName = dataPointDto.DeviceName,
                    Precipitation = dataPointDto.Precipitation,
                    Time = dataPointDto.Time,
                    Latitude = dataPointDto.Latitude,
                    Longitude = dataPointDto.Longitude,
                    Temperature = dataPointDto.Temperature,
                    AtmosphericPressure = dataPointDto.AtmosphericPressure,
                    MaxWindSpeed = dataPointDto.MaxWindSpeed,
                    SolarRadiation = dataPointDto.SolarRadiation,
                    VaporPressure = dataPointDto.VaporPressure,
                    Humidity = dataPointDto.Humidity,
                    WindDirection = dataPointDto.WindDirection,
                    Id = ObjectId.Empty // Set Id to ObjectId.Empty to let MongoDB generate it
                };
                dataPoints.Add(dataPoint);
            }
            _dataPointRepository.InsertMultipleRecords(dataPoints);

            return Ok(dataPoints);
        }


        /// <summary>
        /// Updates a single data point in the database.
        /// </summary>
        /// <param name="id">The ID of the data point to be updated.</param>
        /// <param name="dataPoint">The updated data point.</param>
        /// <returns>An IActionResult indicating the result of the update operation.</returns>
        // PUT: api/DataPoint/{id}
        [HttpPut("{id}")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(201, "Returns success message if the record is updated successfully")]
        [SwaggerResponse(400, "If invalid ID format or ID not provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no data point is found with the provided ID")]
        public IActionResult UpdateSingleRecord(string id, [FromBody] DataPoint dataPoint)
        {
            // check if provided id is null or empty
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID not provided");
            }

            // Check if provided id matches correct objectid pattern
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format");
            }

            try
            {
                // Check if a record with the specified ID exists in the repository
                var existingRecord = _dataPointRepository.GetSingleRecord(id);
                if (existingRecord == null)
                {
                    // Happens if the id passes input verification but does not exist in the database
                    return NotFound("The specified ID does not exist");
                }

                // Update the record
                _dataPointRepository.UpdateSingleRecord(id, dataPoint);
                return Ok("Record updated successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates the precipitation value for a single data point in the database.
        /// </summary>
        /// <param name="id">The ID of the data point to be updated.</param>
        /// <param name="newPrecipitationValue">The new precipitation value.</param>
        /// <returns>An IActionResult indicating the result of the update operation.</returns>
        // PATCH: api/DataPoint/{id}/precipitation
        [HttpPatch("{id}/precipitation")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(201, "Returns success message if the precipitation is updated successfully")]
        [SwaggerResponse(400, "If invalid ID format, ID not provided, or negative precipitation value provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        public IActionResult UpdatePrecipitation(string id, [FromBody] double newPrecipitationValue)
        {
            // Check if provided id is null or empty
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID not provided");
            }

            // Check if provided id matches correct objectid pattern
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format");
            }

            // Additional validation for newPrecipitationValue
            if (newPrecipitationValue < 0)
            {
                return BadRequest("Precipitation value must be non-negative");
            }

            _dataPointRepository.UpdatePrecipitation(id, newPrecipitationValue);
            return Ok("Precipitation updated successfully");
        }

        /// <summary>
        /// Retrieves the data point with the maximum temperature value within the specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>An ActionResult containing the data point if found, or BadRequest if invalid start date or end date format is provided, or NotFound if no data point is found within the specified date range.</returns>
        // GET: api/DataPoint/max-temperature
        [HttpGet("max-temperature")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER, UserRolesEnum.STUDENT)]
        [SwaggerResponse(200, "Returns the data point if found", typeof(DataPoint))]
        [SwaggerResponse(400, "If invalid start date or end date format is provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no data point is found within the specified date range")]
        public ActionResult<DataPoint> GetRecordWithMaxTemperature(DateTime startDate, DateTime endDate)
        {
            // Check if startDate and endDate are provided
            if (startDate == default || endDate == default)
            {
                return BadRequest("Both startDate and endDate must be provided");
            }

            // Check if startDate is before endDate
            if (startDate >= endDate)
            {
                return BadRequest("startDate must be before endDate");
            }

            var maxTemperatureRecord = _dataPointRepository.GetRecordWithMaxTemperature(startDate, endDate);
            if (maxTemperatureRecord == null)
            {
                return NotFound();
            }
            return Ok(maxTemperatureRecord);
        }

        /// <summary>
        /// Retrieves the data point with the maximum precipitation value.
        /// </summary>
        /// <param name="deviceName">The name of the device.</param>
        /// <returns>An ActionResult containing the data point if found, or NotFound if no data point with maximum precipitation is found.</returns>
        // GET: api/DataPoint/max-precipitation
        [HttpGet("max-precipitation")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER, UserRolesEnum.STUDENT)]
        [SwaggerResponse(200, "Returns the data point if found", typeof(MaxPrecipitationRecordDTO))]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no data point with maximum precipitation is found")]
        public ActionResult<MaxPrecipitationRecordDTO> GetMaxPrecipitationRecord(string deviceName)
        {
            var maxPrecipitationRecord = _dataPointRepository.GetMaxPrecipitationRecord(deviceName);

            if (maxPrecipitationRecord == null)
            {
                return NotFound();
            }

            return Ok(maxPrecipitationRecord);
        }

        /// <summary>
        /// Retrieves a snapshot of sensor data for the specified device and snapshot date/time.
        /// </summary>
        /// <param name="deviceName">The name of the sensor device.</param>
        /// <param name="snapShotDateTime">The date/time of the snapshot.</param>
        /// <returns>An ActionResult containing the sensor snapshot if found, BadRequest if device name is not provided, or NotFound if no sensor snapshot is found.</returns>
        // GET: api/DataPoint/snapshot/{deviceName}
        [HttpGet("snapshot/{deviceName}")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER, UserRolesEnum.STUDENT)]
        [SwaggerResponse(200, "Returns the sensor snapshot if found", typeof(DeviceSnapShotDTO))]
        [SwaggerResponse(400, "If device name is not provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no sensor snapshot is found")]
        public ActionResult<DeviceSnapShotDTO> GetSensorSnapShot(string deviceName, [FromQuery] DateTime snapShotDateTime)
        {
            // Check if deviceName is provided
            if (string.IsNullOrEmpty(deviceName))
            {
                return BadRequest("Device name must be provided");
            }

            // Call repository method to get sensor snapshot
            var dataPoint = _dataPointRepository.GetSensorSnapShot(deviceName, snapShotDateTime);

            // Check if snapshot exists
            if (dataPoint == null)
            {
                return NotFound();
            }

            // Map DataPoint to DeviceSnapShotDTO
            var snapshotDTO = new DeviceSnapShotDTO
            {
                DeviceName = dataPoint.DeviceName,
                Precipitation = dataPoint.Precipitation,
                Time = dataPoint.Time,
                Temperature = dataPoint.Temperature,
                AtmosphericPressure = dataPoint.AtmosphericPressure,
                SolarRadiation = dataPoint.SolarRadiation
            };

            return Ok(snapshotDTO);
        }

    }
}
