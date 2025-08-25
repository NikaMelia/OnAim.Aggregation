namespace OnAim.Aggregation.Domain.Entities;

public class AggregationResult
{
    public Guid Id { get; set; }
    public string PlayerId { get; set; }
    public string ProviderService { get; set; }
    public string EventType { get; set; }
    public string SubscriberKey { get; set; }
    public decimal AccumulatedAmount { get; set; }
    public decimal TotalPoints { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}