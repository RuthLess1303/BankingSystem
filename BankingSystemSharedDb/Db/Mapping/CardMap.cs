using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemSharedDb.Db.Mapping;

public class CardMap : IEntityTypeConfiguration<CardEntity>
{
    public void Configure(EntityTypeBuilder<CardEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.CardNumber).IsRequired().HasMaxLength(16).IsFixedLength();
        builder.Property(c => c.NameOnCard).IsRequired().HasMaxLength(40);
        builder.Property(c => c.Cvv).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(c => c.Pin).IsRequired().HasMaxLength(4).IsFixedLength();
        builder.Property(c => c.ExpirationDate).IsRequired();
        builder.Property(c => c.CreationDate).IsRequired();

        // builder.HasOne(x => x.User)
        //     .WithMany(x => x.Cards)
        //     .HasForeignKey(x => x.UserEntityId)
        //     .HasPrincipalKey(x => x.Id)
        //     .OnDelete(DeleteBehavior.Cascade);
        //
        // builder.HasOne(x => x.Account)
        //     .WithMany(x => x.Cards)
        //     .HasForeignKey(x => x.AccountEntityId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}