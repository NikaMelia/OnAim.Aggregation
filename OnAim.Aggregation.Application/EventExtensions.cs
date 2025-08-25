using System.Reflection;
using OnAim.Aggregation.Application.Messages;

namespace OnAim.Aggregation.Application;

public static class EventExtensions
{
    public static string GetEventTopic(this object evt)
    {
        var attr = evt.GetType().GetCustomAttribute<EventTopicAttribute>();
        if (attr is null)
            throw new InvalidOperationException(
                $"Event type {evt.GetType().Name} is missing [EventTopic].");
        return attr.Name;
    }
}