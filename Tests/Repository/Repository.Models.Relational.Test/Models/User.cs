using System.Text.Json;
using Repository.Contracts;

namespace Repository.Models.Relational.Test.Models
{
    public class User : IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this).Replace(",", ",\n");
        }
    }
}
