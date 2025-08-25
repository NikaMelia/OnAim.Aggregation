using DotNetCore.CAP;
using OnAim.Aggregation.Application;

namespace OnAim.Aggregation.Infrastructure;

public sealed class CapEventBus : IEventBus
{
    private readonly ICapPublisher _cap;
    private readonly ITopicNameResolver _resolver;

    public CapEventBus(
        ICapPublisher cap,
        ITopicNameResolver resolver)
    {
        _cap = cap;
        _resolver = resolver;
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        var topic = _resolver.Resolve(typeof(T));
        return _cap.PublishAsync(topic, @event, cancellationToken: ct);
    }
}