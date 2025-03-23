using System.Security.Claims;

namespace Front.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;

        public bool IsAdmin => Role == Consts.AdminRoleName;

        public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(
        [
            new (ClaimTypes.Name, Name),
            new (ClaimTypes.Role, Role),
            new (ClaimTypes.Email, Email),
            new (ClaimTypes.Sid, Id.ToString())
        ],
        Consts.AuthTypeName));

        public static User FromClaimsPrincipal(ClaimsPrincipal principal)
        {
            var guidStr = principal.FindFirst(ClaimTypes.Sid)?.Value ?? throw new ArgumentException(nameof(ClaimTypes.Sid));
            return new()
            {
                Name = principal.FindFirst(ClaimTypes.Name)?.Value ?? throw new ArgumentException(nameof(ClaimTypes.Name)),
                Role = principal.FindFirst(ClaimTypes.Role)?.Value ?? throw new ArgumentException(nameof(ClaimTypes.Role)),
                Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? throw new ArgumentException(nameof(ClaimTypes.Email)),
                Id = Guid.TryParse(guidStr, out Guid value) ? value : Guid.Empty
            };
        }

        public override bool Equals(object? obj)
        {
            if (obj is User uObj)
            {
                return uObj.Email == Email && uObj.Id == Id && uObj.Role == Role;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Role, Id);
        }
    }
}
