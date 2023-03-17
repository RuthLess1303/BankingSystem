using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db;

public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }
    public DbSet<UserEntity> User { get; set; }
    public DbSet<AccountEntity> Account { get; set; }
    public DbSet<CardEntity> Card { get; set; }
    public DbSet<CardAccountConnectionEntity> CardAccountConnection { get; set; }
    public DbSet<TransactionEntity> Transaction { get; set; }
    public DbSet<CurrencyEntity> Currency { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new AccountMap());
        builder.ApplyConfiguration(new CardMap());
        builder.ApplyConfiguration(new CurrencyMap());
        builder.ApplyConfiguration(new TransactionMap());
        builder.ApplyConfiguration(new UserMap());
        builder.ApplyConfiguration(new CardAccountConnectionMap());
        
        base.OnModelCreating(builder);
        
        builder.Entity<RoleEntity>().HasData(new[]
        {
            new RoleEntity {  Id = Guid.NewGuid(),  Name = "api-user", NormalizedName = "API-USER"},
            new RoleEntity {  Id = Guid.NewGuid(),  Name = "api-operator", NormalizedName = "API-OPERATOR"}
        });
        
        var userName = "operator@bank.com";
        var password = "abc123";
        var operatorUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = userName,
            UserName = userName,
            FirstName = "example",
            LastName = "exampleLastname",
            PrivateNumber = "01000000003"
        };
        
        var hasher = new PasswordHasher<UserEntity>();
        operatorUser.PasswordHash = hasher.HashPassword(operatorUser, password);
        builder.Entity<UserEntity>().HasData(operatorUser);
        
        builder.Entity<IdentityUserRole<Guid>>().HasData(new []
        {
            new IdentityUserRole<Guid> { UserId = operatorUser.Id, RoleId = Guid.NewGuid() }
        });
    }
}