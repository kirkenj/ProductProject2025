using Repository.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Core.Domain.Models
{
    [Table(nameof(User) + "s")]
    public class User : IIdObject<Guid>
    {
        [Key]
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string HashAlgorithm { get; set; } = null!;
        public string StringEncoding { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleID { get; set; }
        [ForeignKey(nameof(RoleID))]
        public Role Role { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is User uObj)
            {
                return
                    uObj.Id == Id &&
                    uObj.Login == Login &&
                    uObj.Email == Email &&
                    uObj.Name == Name &&
                    uObj.Address == Address &&
                    uObj.HashAlgorithm == HashAlgorithm &&
                    uObj.StringEncoding == StringEncoding &&
                    uObj.PasswordHash == PasswordHash &&
                    uObj.RoleID == RoleID;
            }


            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var part1 = HashCode.Combine(Id, Login, Email, Name, Address, HashAlgorithm);
            var part2 = HashCode.Combine(StringEncoding, PasswordHash, RoleID);
            return HashCode.Combine(part1, part2);
        }
    }
}
