using System.Collections.Concurrent;
using System.Reflection;
using OnAim.Aggregation.Application.Messages;

namespace OnAim.Aggregation.Infrastructure;

public sealed class TopicNameResolver : ITopicNameResolver
{
    private static readonly ConcurrentDictionary<Type, string> Cache = new();

    public string Resolve(Type eventType) =>
        Cache.GetOrAdd(eventType, t =>
        {
            var attr = t.GetCustomAttribute<EventTopicAttribute>();
            if (attr is null || string.IsNullOrWhiteSpace(attr.Name))
                throw new InvalidOperationException(
                    $"Missing [EventTopic(\"…\")] on {t.FullName}.");
            return attr.Name!;
        });
}