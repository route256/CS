using WeatherSimulator.Server.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace WeatherSimulator.Server.Configurations;

/// <summary>
/// Настройки погодного сервера 
/// </summary>
public class WeatherServerConfiguration
{
    /// <summary>
    /// Настройки сенсоров
    /// </summary>
    public SensorConfiguration[] Sensors { get; set; } = Array.Empty<SensorConfiguration>();
}

/// <summary>
/// Конфигурация для сенсора
/// </summary>
public class SensorConfiguration
{
    /// <summary>
    /// Идентификатор датчика
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Частота опроса сенсора (в мс)
    /// </summary>
    [Range(Constants.Sensors.PoolingFrequencyMin, Constants.Sensors.PoolingFrequencyMax)]
    public int PollingFrequency { get; set; }

    /// <summary>
    /// Тип расположения сенсора (внутри/снаружи)
    /// </summary>
    /// <value></value>
    public Models.Enums.SensorLocationType LocationType { get; set; }
}
