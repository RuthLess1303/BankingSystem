using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemSharedDb.Db.Mapping;

public class CardAccountConnectionMap : IEntityTypeConfiguration<CardAccountConnectionEntity>
{
    public void Configure(EntityTypeBuilder<CardAccountConnectionEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.CardId).IsRequired();
        builder.Property(c => c.Iban).IsRequired().HasMaxLength(36);
        builder.Property(c => c.Hash).IsRequired();
        builder.Property(c => c.CreationDate).IsRequired();

        // builder.HasOne(c => c.Card)
        //     .WithMany();
        // builder.HasOne(c => c.Account)
        //     .WithMany();

        builder.HasOne(x => x.Card)
            .WithMany()
            .HasForeignKey(x => x.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.Iban)
            .OnDelete(DeleteBehavior.Cascade);
    }
}