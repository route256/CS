using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherSimulator.Server.Models;
using WeatherSimulator.Server.Models.Enums;
using WeatherSimulator.Server.Services.Abstractions;
using WeatherSimulator.Server.Storages.Abstractions;

namespace WeatherSimulator.Server.Services.Background;

public class SensorPoolingService : IHostedService, IDisposable
{
    private readonly ILogger<SensorPoolingService> _logger;
    private readonly ConcurrentBag<Timer> _timers = new();
    private readonly IMeasureService _measureService;

    public SensorPoolingService(IMeasureService measureService,
        ILogger<SensorPoolingService> logger)
    {
        _logger = logger;
        _measureService = measureService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var registeredSensors = _measureService.GetAvailableSensors().ToArray();

        _logger.LogInformation("Старт сервиса, опрашивающего {sensorsCount} сенсоров.", registeredSensors.Length);

        for(var i = 0; i < registeredSensors.Length; i++)
        {
            var sensorItem = registeredSensors[i];
            _timers.Add(new Timer(PoolSensor, 
                sensorItem, 
                TimeSpan.Zero, 
                TimeSpan.FromMilliseconds(sensorItem.PollingFrequency)));
        }

        return Task.CompletedTask;
    }

    private void PoolSensor(object? state)
    {
        var sensorInfo = state as Sensor;
        var randGen = new Random();
        
        if (sensorInfo == null)
            return;

        var measureInfo = new SensorMeasure(sensorInfo.Id, 
            temperature: randGen.Next(200, 320) / 10,
            humidity: randGen.Next(40, 60),
            co2: sensorInfo.LocationType == SensorLocationType.External ? randGen.Next(350, 360) : randGen.Next(400, 600));

        _measureService.OnNewMeasure(measureInfo);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        foreach(var timer in _timers)
        {
            timer?.Change(Timeout.Infinite, 0);
        }  

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        foreach(var timer in _timers)
        {
            timer?.Dispose();
        }
    }
}
