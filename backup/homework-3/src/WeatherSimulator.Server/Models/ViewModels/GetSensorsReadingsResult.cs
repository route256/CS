using System;
using System.Collections.Generic;
using WeatherSimulator.Server.Models.Enums;

namespace WeatherSimulator.Server.Models.ViewModels
{
    public class GetSensorsReadingsResult
    {
        public IReadOnlyCollection<GetSensorsReadingsResultItem> Items { get; init; } =
            Array.Empty<GetSensorsReadingsResultItem>();
    }

    public class GetSensorsReadingsResultItem
    {
        /// <summary>
        /// Идентификатор сенсора
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Температура
        /// </summary>
        public double Temperature { get; init; }

        /// <summary>
        /// Влажность
        /// </summary>
        public int Humidity { get; init; }

        /// <summary>
        /// Показатель CO2
        /// </summary>
        public int CO2 { get; init; }

        /// <summary>
        /// Тип расположения сенсора (внутренний/внешний)
        /// </summary>
        public SensorLocationType LocationType { get; init; }
    }
}