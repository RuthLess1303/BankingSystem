using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBank.Db.Db.Mapping;

public class AccountMap : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.PrivateNumber).IsRequired().HasMaxLength(11).IsFixedLength();
        builder.Property(a => a.Iban).IsRequired().HasMaxLength(36);
        builder.Property(a => a.CurrencyCode).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(a => a.Balance).IsRequired();
        builder.Property(a => a.CreationDate).IsRequired();
    }
}