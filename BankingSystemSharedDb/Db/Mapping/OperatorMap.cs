using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingSystemSharedDb.Db.Mapping;

public class OperatorMap : IEntityTypeConfiguration<OperatorEntity>
{
    public void Configure(EntityTypeBuilder<OperatorEntity> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Surname).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PrivateNumber).IsRequired().HasMaxLength(11).IsFixedLength();
        builder.Property(c => c.BirthDate).IsRequired();
        builder.Property(c => c.Email).IsRequired().HasMaxLength(320);
        builder.Property(c => c.Password).IsRequired().HasMaxLength(100);
        builder.Property(c => c.CreationDate).IsRequired();
    }
}