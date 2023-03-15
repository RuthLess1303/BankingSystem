using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemSharedDb.Db.Mapping;

public class UserMap : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.BirthDate).IsRequired();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PrivateNumber).IsRequired().HasMaxLength(11).IsFixedLength();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(320);
        builder.Property(u => u.CreationDate).IsRequired();

        builder.HasMany(u => u.Accounts)
            .WithOne(a => a.user)
            .HasForeignKey(a => a.UserEntityId);

        builder.HasMany(u => u.Cards)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserEntityId);
    }
}