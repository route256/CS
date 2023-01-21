using System;
using WeatherSimulator.Server.Models.Enums;

namespace WeatherSimulator.Server.Models;

/// <summary>
/// Информация приходящая с датчика
/// </summary>
public class Sensor
{
    public Sensor(Guid id, 
        int poolingFrequency,
        SensorLocationType locationType)
    {
        Id = id;
        PollingFrequency = poolingFrequency;
        LocationType = locationType;
    }

    /// <summary>
    /// Идентификатор сенсора
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Тип расположения сенсора (внутренний/внешний)
    /// </summary>
    public SensorLocationType LocationType { get; }

    public int PollingFrequency { get; set; }
}
