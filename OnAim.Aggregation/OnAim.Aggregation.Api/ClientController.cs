using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnAim.Aggregation.Api.Dtos;
using OnAim.Aggregation.Application;
using OnAim.Aggregation.Application.Messages.PlayerEvents;
using OnAim.Aggregation.Application.Services.AggregationConfiguration;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Api;

[ApiController]
[Route("[controller]")]
public class ClientController : Controller
{
    private readonly IAggregationConfigure _aggregationConfigure;
    private readonly IMapper _mapper;
    private readonly IEventBus _eventBus;

    public ClientController(
        IAggregationConfigure aggregationConfigure,
        IMapper mapper, IEventBus eventBus)
    {
        _aggregationConfigure = aggregationConfigure;
        _mapper = mapper;
        _eventBus = eventBus;
    }

    [HttpPost("configs")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfigureAggregation([FromBody] CreateAggregationConfigRequest request, CancellationToken ct)
    {
        try
        {
            AggregationConfig? aggregationConfig = _mapper.Map<AggregationConfig>(request);
            var result = await _aggregationConfigure.AddAggregationConfig(aggregationConfig, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("produce/bet-placed")]
    public async Task<IActionResult> ProduceBetPlacedEvent([FromBody] BetPlacedEvent body, CancellationToken ct)
    {
        await _eventBus.PublishAsync(body, ct);
        return Ok(new { published = true });
    }


    [HttpPost("produce/deposit-made")]
    public async Task<IActionResult> ProduceDepositMadeEvent([FromBody] DepositMadeEvent body, CancellationToken ct)
    {
        await _eventBus.PublishAsync(body, ct);
        return Ok(new { published = true });
    }
}