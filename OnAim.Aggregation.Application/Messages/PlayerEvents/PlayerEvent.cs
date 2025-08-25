namespace OnAim.Aggregation.Application.Messages.PlayerEvents;

public class PlayerEvent
{
    public string Provider { get; set; }
    public string PlayerId { get; set; }
    public decimal Amount { get; set; }
}