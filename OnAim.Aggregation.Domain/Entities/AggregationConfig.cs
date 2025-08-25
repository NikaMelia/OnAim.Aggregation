using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnAim.Aggregation.Domain.Entities;

public class AggregationConfig
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; } // external id string
    public string Provider { get; set; } // emitting service
    public string Subscriber { get; set; } // consumer of points
    public string Owner { get; set; } // config owner (who created)
    public string SubscriberKey { get; set; } // to tag aggregated events
    public string EventType { get; set; }  // event type to match
    public List<FilterClause> Filters { get; set; } = new();
    public AggregationRule Rule { get; set; } // e.g., "50:1" or "50"
    public bool IsEnabled { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

public class FilterClause
{
    public string Field { get; set; } // e.g., "playerId", "currency", "metadata.country"
    public FilterOperator Op { get; set; } // Eq, Gte, In, etc.
    /// <summary>
    /// Value to compare against. Stored as Bson for flexibility (string, number, bool, array).
    /// </summary>
    public BsonValue Value { get; set; } = BsonNull.Value;

    public static FilterClause CreateFilterClause(string field, FilterOperator op, BsonValue obj)
    {
        return new FilterClause()
        {
            Field = field,
            Op = op,
            Value = obj
        };
    }
}

public enum FilterOperator
{
    /// <summary>
    /// equals
    /// </summary>
    Eq = 0,

    /// <summary>
    /// not equals
    /// </summary>
    Ne = 1,

    /// <summary>
    /// greater than
    /// </summary>
    Gt = 2,

    /// <summary>
    /// less than
    /// </summary>
    Lt = 3,

    /// <summary>
    /// regex matches
    /// </summary>
    Regex = 5
}