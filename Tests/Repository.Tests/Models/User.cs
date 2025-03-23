using Repository.Contracts;
using System.Text.Json;

namespace Repository.Tests.Models
{
    public class User : IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is User u)
            {
                return
                    this.Email == u.Email &&
                    this.Name == u.Name &&
                    this.Address == u.Address &&
                    this.Id == u.Id &&
                    this.Login == u.Login;
            }

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this).Replace(",", ",\n");
        }
    }
}
