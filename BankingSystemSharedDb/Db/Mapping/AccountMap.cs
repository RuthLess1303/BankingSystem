using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemSharedDb.Db.Mapping;

public class AccountMap : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.PrivateNumber).IsRequired().HasMaxLength(11).IsFixedLength();
        builder.Property(a => a.Iban).IsRequired().HasMaxLength(36);
        builder.Property(a => a.CurrencyCode).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(a => a.Balance).IsRequired();
        builder.Property(a => a.Hash).IsRequired();
        builder.Property(a => a.CreationDate).IsRequired();

        builder.HasOne(a => a.user)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.PrivateNumber);

        builder.HasMany(a => a.Cards)
            .WithOne(c => c.Account)
            .HasForeignKey(c => c.AccountEntityId);

        builder.HasOne(x => x.user)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.PrivateNumber)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.IncomingTransactions)
            .WithOne(x => x.Receiver)
            .HasForeignKey(x => x.ReceiverIban);
        
        builder.HasMany(x => x.OutgoingTransactions)
            .WithOne(x => x.Aggressor)
            .HasForeignKey(x => x.AggressorIban);
    }
}