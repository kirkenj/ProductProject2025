using AuthService.Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<User>().Navigation(u => u.Role).AutoInclude();

            Role adminRole = new() { Id = 1, Name = "Admin" };
            Role regularRole = new() { Id = 2, Name = "Regular" };
            modelBuilder.Entity<Role>().HasData(adminRole);
            modelBuilder.Entity<Role>().HasData(regularRole);

            string hashAlgorithmName = "MD5";

            HashAlgorithm hashAlgorithm = MD5.Create() ?? throw new Exception($"Hash algorithm not found {hashAlgorithmName}");
            Encoding encoding = Encoding.UTF8;

            ((string login, string password, string name, string email) userData, int roleID)[] startUsersArray = new[]
            {
                (("admin", "admin", "seeding admin", "admin@product"), adminRole.Id),
                (("user", "user", "seeding user", "user@product"), regularRole.Id)
            };

            foreach (var item in startUsersArray)
            {
                var pwdBytes = encoding.GetBytes(item.userData.password);
                var pwdHash = hashAlgorithm.ComputeHash(pwdBytes);
                var pwdHashString = encoding.GetString(pwdHash);
                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = Guid.NewGuid(),
                    Login = item.userData.login,
                    RoleID = item.roleID,
                    Email = item.userData.email,
                    Name = item.userData.name,
                    Address = "Confidential",
                    PasswordHash = pwdHashString,
                    StringEncoding = encoding.BodyName,
                    HashAlgorithm = hashAlgorithmName
                });
            }


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        }
    }
}
