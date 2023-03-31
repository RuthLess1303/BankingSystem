using InternetBank.Db.Db;
using Microsoft.EntityFrameworkCore;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer("Server=localhost;Database=MobileBankDb;User Id=sa; Password=wavedi123;Encrypt=False;");