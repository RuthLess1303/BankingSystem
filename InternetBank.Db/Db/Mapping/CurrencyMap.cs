using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class CurrencyMap : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasAlternateKey(t => t.Code);
        builder.Property(t => t.Code).IsRequired();
        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.Diff).IsRequired();
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Quantity).IsRequired();
        builder.Property(t => t.Rate).IsRequired();
        builder.Property(t => t.DiffFormatted).IsRequired();
        builder.Property(t => t.RateFormatted).IsRequired();
        builder.Property(t => t.ValidFromDate).IsRequired();
    }
}