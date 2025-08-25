using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using OnAim.Aggregation.Application.Messages.PlayerEvents;
using OnAim.Aggregation.Application.Repositories;
using OnAim.Aggregation.Application.Services.AmountAggregation;
using OnAim.Aggregation.Application.Transactions;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Services.EventHandlers;

public class PlayerEventsHandler : ICapSubscribe
{
    private readonly IAggregationConfigRepository _configRepository;
    private readonly IAggregationResultsRepository _aggregationResultsRepository;
    private readonly IAmountAggregator _amountAggregator;
    private readonly ITransactionalSession _transactionalSession;

    public PlayerEventsHandler(
        IAggregationConfigRepository configRepository,
        IAggregationResultsRepository aggregationResultsRepository,
        IAmountAggregator amountAggregator,
        ITransactionalSession transactionalSession)
    {
        _configRepository = configRepository;
        _aggregationResultsRepository = aggregationResultsRepository;
        _amountAggregator = amountAggregator;
        _transactionalSession = transactionalSession;
    }

    [CapSubscribe("players.bet-placed")]
    public async Task Handle(BetPlacedEvent @event, CancellationToken ct)
    {
        AggregationConfig? configuration =
            await _configRepository.GetActiveAsync(@event.Provider, @event.GetEventTopic(), ct);

        if (configuration is null)
        {
            return;
        }

        if (!EventFilter.Matches(@event, configuration.Filters))
        {
            return;
        }

        IDbContextTransaction transaction = await _transactionalSession.BeginTransactionAsync(ct);
        await AggregateEvents(configuration, @event);
        await transaction.CommitAsync(ct);
    }

    [CapSubscribe("players.deposit-made")]
    public async Task Handle(DepositMadeEvent @event, CancellationToken ct)
    {
        AggregationConfig? configuration =
            await _configRepository.GetActiveAsync(@event.Provider, @event.GetEventTopic(), ct);

        if (configuration is null)
        {
            return;
        }

        if (!EventFilter.Matches(@event, configuration.Filters))
        {
            return;
        }

        IDbContextTransaction transaction = await _transactionalSession.BeginTransactionAsync(ct);
        await AggregateEvents(configuration, @event);
        await transaction.CommitAsync(ct);
    }

    private async Task AggregateEvents(AggregationConfig configuration, PlayerEvent @event)
    {
        AggregationResult? result =
            await _aggregationResultsRepository.GetByPlayerId(@event.PlayerId, @event.Provider, @event.GetEventTopic());

        if (result is null)
        {
            result = new AggregationResult
            {
                Id = Guid.NewGuid(),
                PlayerId = @event.PlayerId,
                ProviderService = @event.Provider,
                EventType = @event.GetEventTopic(),
                SubscriberKey = configuration.SubscriberKey,
                AccumulatedAmount = @event.Amount,
                TotalPoints = 0,
            };
            result = await _aggregationResultsRepository.Add(result);
            result =  await _amountAggregator.AggregateAmount(result, configuration, 0);
            await _aggregationResultsRepository.Update(result);
            return;
        }

        result =  await _amountAggregator.AggregateAmount(result, configuration, @event.Amount);
        await _aggregationResultsRepository.Update(result);
    }
}