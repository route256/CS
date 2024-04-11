using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherSimulator.Server.Models;

public class Constants
{
    public class Sensors
    {
        /// <summary>
        /// Минимальное значение частоты опроса датчика в мс
        /// </summary>
        public const int PoolingFrequencyMin = 100;
        
        /// <summary>
        /// Максимальное значение частоты опроса датчика в мс
        /// </summary>
        public const int PoolingFrequencyMax = 2000;
    }
}
