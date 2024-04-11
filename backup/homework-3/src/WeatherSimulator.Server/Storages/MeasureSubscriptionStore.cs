using System;
using System.Collections.Generic;
using System.Linq;
using WeatherSimulator.Server.Storages.Abstractions;
using WeatherSimulator.Server.Models;
using System.Collections.Concurrent;
using System.Threading;

namespace WeatherSimulator.Server.Storages;

public class MeasureSubscriptionStore : IMeasureSubscriptionStore
{
	private readonly object locker = new();
	private readonly ConcurrentDictionary<Guid, HashSet<SensorMeasureSubscription>> subscriptionsDict = new();

	public IReadOnlyCollection<SensorMeasureSubscription> GetSubscriptions(Guid sensorId)
	{
		if (!subscriptionsDict.TryGetValue(sensorId, out var sensorSubscription))
		{
			return Array.Empty<SensorMeasureSubscription>();
		}

		SensorMeasureSubscription[] items;
		lock (locker)
		{
			items = sensorSubscription.ToArray();
		}

		return items;
	}

	public void RemoveSubscription(Guid sensorId, Guid subscriptionId)
	{
		if (!subscriptionsDict.TryGetValue(sensorId, out var sensorSubscription))
		{
			return;
		}

		//HACK: Подписки равные, если у них одинаковый id и sensorId. Создаем фейковую подписку, чтобы удалить настоящую из set-а.
		SensorMeasureSubscription sub = new(subscriptionId, sensorId, CancellationToken.None, null!);

		lock (locker)
		{
			sensorSubscription.Remove(sub);
		}
	}

	public void AddSubscription(SensorMeasureSubscription subscription)
	{
		if (subscriptionsDict.TryGetValue(subscription.SensorId, out var sensorSubscription))
		{
			lock (locker)
			{
				sensorSubscription.Add(subscription);
				return;
			}
		}

		sensorSubscription = new HashSet<SensorMeasureSubscription>
		{
			subscription
		};

		subscriptionsDict.AddOrUpdate(subscription.SensorId,
			sensorSubscription,
			(_, set) =>
			{
				set.Add(subscription);
				return set;
			});
	}
}
