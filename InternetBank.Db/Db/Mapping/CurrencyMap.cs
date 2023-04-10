using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class CurrencyMap : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Code).IsRequired();
        builder.Property(t => t.Code).IsRequired();
        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.Diff).IsRequired().HasPrecision(18,6);
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Quantity).IsRequired();
        builder.Property(t => t.Rate).IsRequired().HasPrecision(18,6);
        builder.Property(t => t.DiffFormatted).IsRequired().HasPrecision(18,6);
        builder.Property(t => t.RateFormatted).IsRequired().HasPrecision(18,6);
        builder.Property(t => t.ValidFromDate).IsRequired();
        builder.Property(t => t.RatePerQuantity).IsRequired().HasPrecision(18,6);
    }
}