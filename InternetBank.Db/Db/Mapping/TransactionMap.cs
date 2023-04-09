using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class TransactionMap : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).IsRequired();
        builder.Property(t => t.GrossAmount).IsRequired();
        builder.Property(t => t.SenderIban).IsRequired().HasMaxLength(36);
        builder.Property(t => t.ReceiverIban).IsRequired().HasMaxLength(36);
        builder.Property(t => t.CurrencyCode).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(t => t.Fee).IsRequired();
        builder.Property(t => t.Type).IsRequired();
        builder.Property(t => t.Rate).IsRequired();
        builder.Property(t => t.TransactionTime).IsRequired();
    }
    
}