namespace OnAim.Aggregation.Application.Services.ClientServerEventProducer;

public interface IRandomEventProducer
{
    public Task ProduceRandomEvent(CancellationToken cancellationToken);
}