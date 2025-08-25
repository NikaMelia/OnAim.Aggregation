using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OnAim.Aggregation.Api.Dtos;

public sealed class CreateAggregationConfigRequest
{
    [Required]
    public string Provider { get; init; }
    [Required]
    public string Subscriber { get; init; }
    [Required]
    public string Owner { get; init; }
    [Required]
    public string SubscriberKey { get; init; }
    [Required]
    public string EventType { get; init; }
    public List<FilterClauseDto>? Filters { get; init; }
    [Required] public AggregationRuleDto Rule { get; init; }
    public bool IsEnabled { get; init; }
}

public sealed class AggregationRuleDto
{
    public decimal PointsThreshold { get; init; }
    public int PointsAwarded { get; init; } = 1;
}

public class FilterClauseDto
{
    public string Field { get; set; }
    public FilterOperator Op { get; set; }
    public JsonElement Value { get; set; }
}

public enum FilterOperator
{
    Eq, Ne, Gt, Lt, Regex
}