using System.Collections.Concurrent;
using System.Collections.Immutable;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.ServiceDiscovery.Configuration;

namespace Ozon.Route256.Practice.ServiceDiscovery;

public class ResourceStore : IResourceStore, IDisposable
{
    private readonly ILogger<ResourceStore>                                        _logger;
    private readonly ConcurrentDictionary<string, ImmutableList<CompletionSource>> _streams = new();
    private readonly Timer                                                         _timer;
    private          DbState                                                       _currentState;
    private readonly IDisposable                                                   _updateStateRef;
    private          bool                                                          _disposable;

    public ResourceStore(IOptionsMonitor<DbState> dbStateOption, ILoggerFactory loggerFactory)
    {
        _updateStateRef = dbStateOption.OnChange(UpdateState);
        _currentState   = dbStateOption.CurrentValue;
        _logger         = loggerFactory.CreateLogger<ResourceStore>();
        _timer          = new Timer(SendUpdateData, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    private void UpdateState(DbState state)
    {
        _currentState = state;
    }

    private async void SendUpdateData(object? state)
    {
        foreach (var stream in _streams)
        {
            foreach (var source in stream.Value.Where(s => !s.Task.IsCompletedSuccessfully))
            {
                try
                {
                    var replicas = ConvertToReplicas(_currentState.Clusters[stream.Key]);

                    await source.ResponseStream.WriteAsync(
                        new DbResourcesResponse
                        {
                            ClusterName = stream.Key,
                            Replicas =
                            {
                                replicas
                            },
                            LastUpdated = DateTime.UtcNow.ToTimestamp(),
                        });
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "Ошибка отправки данных в стрим");

                    return;
                }
            }
        }
    }

    private static IEnumerable<Replica> ConvertToReplicas(IEnumerable<ReplicaInfo> replicas)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var replicaInfo in replicas)
        {
            yield return new Replica
            {
                Host = replicaInfo.Host,
                Port = replicaInfo.Port,
                Type = Replica.Types.ReplicaType.Master,
                Buckets =
                {
                    replicaInfo.Buckets
                }
            };
        }
    }

    public void Append(string resource, CompletionSource completionSource)
    {
        _streams.AddOrUpdate(resource, _ => ImmutableList.Create(completionSource), (_, list) => list.Add(completionSource));
        _timer.Change(
            TimeSpan.FromSeconds(0),
            TimeSpan.FromSeconds(Convert.ToDouble(Environment.GetEnvironmentVariable("ROUTE256_UPDATE_TIMEOUT"))));
    }

    public void Remove(string resource, CompletionSource completionSource)
    {
        var spinner = new SpinWait();
        while (_streams.TryGetValue(resource, out var bag) && !_streams.TryUpdate(resource, bag.Remove(completionSource), bag))
            spinner.SpinOnce();
    }

    public bool Contains(string resource)
    {
        return _currentState.Clusters.ContainsKey(resource);
    }

    public void Dispose()
    {
        if (_disposable)
            return;

        _timer.Dispose();
        _updateStateRef.Dispose();
        GC.SuppressFinalize(this);
        _disposable = true;
    }
}