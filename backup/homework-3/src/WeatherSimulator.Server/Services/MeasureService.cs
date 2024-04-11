using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherSimulator.Server.Configurations;
using WeatherSimulator.Server.Models;
using WeatherSimulator.Server.Services.Abstractions;
using WeatherSimulator.Server.Storages.Abstractions;

namespace WeatherSimulator.Server.Services;

public class MeasureService : IMeasureService
{
    private readonly ILogger<MeasureService> logger;
    private readonly IMeasureSubscriptionStore subscriptionStore;
    private readonly WeatherServerConfiguration _weatherServerConfiguration;
    private readonly Dictionary<Guid, Sensor> sensors;

    public MeasureService( 
        IMeasureSubscriptionStore subscriptionStore, 
        IOptions<WeatherServerConfiguration> weatherServerOptions,
        ILogger<MeasureService> logger)
    {
        this.subscriptionStore = subscriptionStore;
        this.logger = logger;
        _weatherServerConfiguration = weatherServerOptions.Value;
        if (_weatherServerConfiguration is null || _weatherServerConfiguration.Sensors.Length == 0)
            throw new Exception("Ни один сенсор не был сконфигурирован. Добавьте конфигурацию для сенсоров");
        if (_weatherServerConfiguration.Sensors.Length < 2)
            throw new Exception("Было сконфигурировано слишком мало сенсоров. Минимум 2");
        var internalSensors = _weatherServerConfiguration.Sensors.Where(it => it.LocationType == Models.Enums.SensorLocationType.Internal).ToArray();
        var externalSensors = _weatherServerConfiguration.Sensors.Where(it => it.LocationType == Models.Enums.SensorLocationType.External).ToArray();
        if (internalSensors.Length < 1)
            throw new Exception("Было сконфигурировано слишком мало внутренних сенсоров. Минимум 1");
        if (externalSensors.Length < 1)
            throw new Exception("Было сконфигурировано слишком мало внешних сенсоров. Минимум 1");

        sensors = new();

        foreach(var weatherSensor in _weatherServerConfiguration.Sensors)
        {
            sensors[weatherSensor.Id] = new Sensor(weatherSensor.Id, weatherSensor.PollingFrequency, weatherSensor.LocationType);
        }
    }

    public void OnNewMeasure(SensorMeasure measure)
    {
        if (!sensors.ContainsKey(measure.SensorId))
        {
            throw new ArgumentException(nameof(measure.SensorId));
        }

        foreach (SensorMeasureSubscription subscription in subscriptionStore.GetSubscriptions(measure.SensorId))
        {
            if (subscription.CancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Removed subscription");
                subscriptionStore.RemoveSubscription(subscription.SensorId, subscription.Id);
                continue;
            }

            Task.Run(async () => await subscription.Callback(measure));
        }
    }

    public Guid SubscribeToMeasures(Guid sensorId, Func<SensorMeasure, Task> callback, CancellationToken cancellationToken)
    {
        if (!sensors.ContainsKey(sensorId))
        {
            throw new Exception($"Sensor with id {sensorId} is not registered");
        }

        var subscription = new SensorMeasureSubscription(Guid.NewGuid(), sensorId, cancellationToken, callback);
        subscriptionStore.AddSubscription(subscription);
        return subscription.Id;
    }

    public void UnsubscribeFromMeasures(Guid sensorId, Guid subscriptionId)
    {
        subscriptionStore.RemoveSubscription(sensorId, subscriptionId);
    }

    public IReadOnlyCollection<Sensor> GetAvailableSensors()
    {
        return sensors.Values;
    }
}