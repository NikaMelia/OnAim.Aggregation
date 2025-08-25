using OnAim.Aggregation.Application.Messages.PlayerEvents;

namespace OnAim.Aggregation.Application.Services.ClientServerEventProducer;

public class RandomEventProducer : IRandomEventProducer
{
    private readonly IEventBus _eventBus;
    private readonly Random _random;

    public RandomEventProducer(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _random = new Random();
    }

    public async Task ProduceRandomEvent(CancellationToken cancellationToken)
    {
        int numberOfPlayer = _random.Next(0, 7);
        for (int i = 0; i < numberOfPlayer; i++)
        {
            int numberOfEvents = _random.Next(0, 10);
            string playerId = Guid.NewGuid().ToString();
            for (int j = 0; j < numberOfEvents; j++)
            {
                BetPlacedEvent betPlacedEvent = new BetPlacedEvent
                {
                    Provider = "ClientA",
                    PlayerId = playerId,
                    Amount = Math.Round((decimal)(_random.NextDouble() * 100), 2),
                    GameId = $"game-{_random.Next(1, 50)}",
                    Currency = "USD",
                    OccuredAt = DateTime.UtcNow
                };
                DepositMadeEvent depositMadeEvent = new DepositMadeEvent()
                {
                    Provider = "ClientA",
                    PlayerId = playerId,
                    Amount = Math.Round((decimal)(_random.Next(20, 200)), 2),
                };
                await _eventBus.PublishAsync(betPlacedEvent, cancellationToken);
                await _eventBus.PublishAsync(depositMadeEvent, cancellationToken);
            }
        }
    }
}