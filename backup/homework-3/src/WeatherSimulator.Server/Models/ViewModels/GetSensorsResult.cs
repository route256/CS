using System;
using System.Collections.Generic;
using WeatherSimulator.Server.Models.Enums;

namespace WeatherSimulator.Server.Models.ViewModels
{
    public class GetSensorsResult
    {
        public IReadOnlyCollection<GetSensorsResultItem> Items { get; set; } = Array.Empty<GetSensorsResultItem>();
    }

    public class GetSensorsResultItem
    {
        /// <summary>
        /// Идентификатор сенсора
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип расположения сенсора (внутренний/внешний)
        /// </summary>
        public SensorLocationType LocationType { get; init; }
    }
}