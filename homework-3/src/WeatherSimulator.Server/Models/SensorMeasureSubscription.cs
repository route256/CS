using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherSimulator.Server.Models;

public class SensorMeasureSubscription : IEquatable<SensorMeasureSubscription>
{
	public SensorMeasureSubscription(Guid id,
		Guid sensorId,
		CancellationToken cancellationToken,
		Func<SensorMeasure, Task> callback)
	{
		Id = id;
		SensorId = sensorId;
		CancellationToken = cancellationToken;
		Callback = callback;
	}

	public Guid Id { get; }

	public Guid SensorId { get; }

	public CancellationToken CancellationToken { get; }

	public Func<SensorMeasure, Task> Callback { get; }

	public bool Equals(SensorMeasureSubscription? other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}
		if (ReferenceEquals(this, other))
		{
			return true;
		}
		return Id.Equals(other.Id)
			&& SensorId.Equals(other.SensorId);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}
		if (ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((SensorMeasureSubscription)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, SensorId);
	}
}