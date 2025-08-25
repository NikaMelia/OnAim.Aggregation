namespace OnAim.Aggregation.Application.Messages.PlayerEvents;

[EventTopic("players.deposit-made")]
public class DepositMadeEvent : PlayerEvent
{
    public string Currency { get; set; }
    // casino name
    public string Aggregator { get; set; }
}