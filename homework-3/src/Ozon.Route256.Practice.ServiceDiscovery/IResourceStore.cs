namespace Ozon.Route256.Practice.ServiceDiscovery;

public interface IResourceStore
{
    void Append(string resource, CompletionSource completionSource);

    void Remove(string resource, CompletionSource completionSource);

    bool Contains(string resource);
}