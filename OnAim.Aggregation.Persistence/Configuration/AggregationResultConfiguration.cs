using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Persistence.Configuration;

public class AggregationResultConfiguration : IEntityTypeConfiguration<AggregationResult>
{
    public void Configure(EntityTypeBuilder<AggregationResult> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAdd();
    }
}