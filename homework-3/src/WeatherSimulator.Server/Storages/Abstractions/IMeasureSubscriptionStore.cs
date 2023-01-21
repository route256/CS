using System;
using System.Collections.Generic;
using WeatherSimulator.Server.Models;

namespace WeatherSimulator.Server.Storages.Abstractions;

public interface IMeasureSubscriptionStore
{
    IReadOnlyCollection<SensorMeasureSubscription> GetSubscriptions(Guid sensorId);

    void RemoveSubscription(Guid sensorId, Guid subscriptionId);

    void AddSubscription(SensorMeasureSubscription subscription);
}