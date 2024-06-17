using System;
using System.Collections.Generic;
using WeatherDataAPI.Models;
using WeatherDataAPI.Models.DTOs;

namespace WeatherDataAPI.Repository
{
    public interface IDataPointRepository
    {
        DataPoint GetSingleRecord(string id);
        IEnumerable<DataPoint> GetRecordsForSensorInRange(string deviceName, DateTime startDate, DateTime endDate);
        void InsertSingleRecord(DataPoint dataPoint);
        void InsertMultipleRecords(IEnumerable<DataPoint> dataPoints);
        void UpdateSingleRecord(string id, DataPoint dataPoint);
        MaxPrecipitationRecordDTO GetMaxPrecipitationRecord(string deviceName);
        List<MaxTemperatureRecordDTO> GetRecordWithMaxTemperature(DateTime startDate, DateTime endDate);
        IEnumerable<DataPoint> GetDataPointsInRange(DateTime startDate, DateTime endDate);
        void UpdatePrecipitation(string id, double newPrecipitationValue);

        DataPoint GetSensorSnapShot(string deviceName, DateTime snapShotDateTime);
    }
}