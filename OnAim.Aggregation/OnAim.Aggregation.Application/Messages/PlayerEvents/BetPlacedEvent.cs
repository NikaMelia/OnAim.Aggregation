namespace OnAim.Aggregation.Application.Messages.PlayerEvents;

[EventTopic("players.bet-placed")]
public class BetPlacedEvent : PlayerEvent
{
    public string GameId { get; set; }
    public string Currency { get; set; }
    public DateTime OccuredAt { get; set; }

    // random fields
}
