using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherSimulator.Server.Models;

namespace WeatherSimulator.Server.Services.Abstractions;

public interface IMeasureService
{
    void OnNewMeasure(SensorMeasure measure);

    Guid SubscribeToMeasures(Guid sensorId, Func<SensorMeasure, Task> callback, CancellationToken cancellationToken);

    void UnsubscribeFromMeasures(Guid sensorId, Guid subscriptionId);

    IReadOnlyCollection<Sensor> GetAvailableSensors();
}
