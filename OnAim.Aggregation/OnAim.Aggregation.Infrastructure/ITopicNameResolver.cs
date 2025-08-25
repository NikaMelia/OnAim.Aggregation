namespace OnAim.Aggregation.Infrastructure;

public interface ITopicNameResolver
{
    string Resolve(Type eventType);
}