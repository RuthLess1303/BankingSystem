using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class CardAccountConnectionMap : IEntityTypeConfiguration<CardAccountConnectionEntity>
{
    public void Configure(EntityTypeBuilder<CardAccountConnectionEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.CardId).IsRequired();
        builder.Property(c => c.Iban).IsRequired().HasMaxLength(36);
        builder.Property(c => c.CreationDate).IsRequired();
    }
}