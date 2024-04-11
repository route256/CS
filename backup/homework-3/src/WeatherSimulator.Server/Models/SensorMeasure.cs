using System;
using WeatherSimulator.Server.Models.Enums;

namespace WeatherSimulator.Server.Models;

public class SensorMeasure
{
    public SensorMeasure(Guid sensorId,
        double temperature, 
        int humidity, 
        int co2)
    {
        SensorId = sensorId;
        Temperature = temperature;
        Humidity = humidity;
        CO2 = co2;
        LastUpdate = DateTime.Now;
    }

    /// <summary>
    /// Идентификатор сенсора
    /// </summary>
    public Guid SensorId { get; private set; }

    /// <summary>
    /// Температура
    /// </summary>
    public double Temperature { get; private set; }

    /// <summary>
    /// Влажность
    /// </summary>
    public int Humidity { get; private set; }
    
    /// <summary>
    /// Показатель CO2
    /// </summary>
    public int CO2 { get; private set; }

    public SensorLocationType LocationType { get; set; }

    /// <summary>
    /// Время последнего обновления сенсора
    /// </summary>
    public DateTime LastUpdate { get; private set; }
}
