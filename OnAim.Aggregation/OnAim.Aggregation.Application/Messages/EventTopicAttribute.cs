namespace OnAim.Aggregation.Application.Messages;

public sealed class EventTopicAttribute : Attribute
{
    public string Name { get; }
    public EventTopicAttribute(string name) => Name = name;
}