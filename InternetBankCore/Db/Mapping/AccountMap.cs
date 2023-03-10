using InternetBankCore.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternetBankCore.Db.Mapping;

public class AccountMap : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.PrivateNumber).IsRequired().HasMaxLength(11).IsFixedLength();
        builder.Property(a => a.Iban).IsRequired().HasMaxLength(36);
        builder.Property(a => a.CurrencyCode).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(a => a.Amount).IsRequired();
        builder.Property(a => a.Hash).IsRequired();
        builder.Property(a => a.CreationDate).IsRequired();
        builder
            .HasOne(a => a.Transactions)
            .WithMany()
            .HasForeignKey(a => a.Iban);
    }
}