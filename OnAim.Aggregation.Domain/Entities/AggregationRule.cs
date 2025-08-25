namespace OnAim.Aggregation.Domain.Entities;

public class AggregationRule
{
    /// <summary>
    /// How much "amount" must be accumulated before points are awarded.
    /// Example: 50 means every 50 amount triggers points.
    /// </summary>
    public decimal PointThreshold { get; set; }

    /// <summary>
    /// How many points to award each time the threshold is crossed.
    /// Default = 1.
    /// </summary>
    public int AwardPoints { get; set; } = 1;

    public static AggregationRule Create(decimal threshold, int awarded = 1)
    {
        if (threshold <= 0) throw new ArgumentOutOfRangeException(nameof(threshold));
        if (awarded <= 0) throw new ArgumentOutOfRangeException(nameof(awarded));
        return new AggregationRule { PointThreshold = threshold, AwardPoints = awarded };
    }
}