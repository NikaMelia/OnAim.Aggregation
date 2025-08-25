namespace OnAim.Aggregation.Application.Messages.AggregatedEvents;

[EventTopic("aggregation.points-accumulated")]
public class PointsAccumulatedEvent
{
    public string PlayerId { get; set; }
    public string Provider { get; set; }
    public decimal Amount { get; set; }
    public string SubscriberKey { get; set; }
}