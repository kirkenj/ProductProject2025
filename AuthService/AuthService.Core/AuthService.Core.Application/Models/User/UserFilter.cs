using System.Diagnostics.CodeAnalysis;

namespace Application.Models.User
{
    public class UserFilter
    {
        [AllowNull]
        public IEnumerable<Guid>? Ids { get; set; }
        [AllowNull]
        public string? AccurateLogin { get; set; }
        [AllowNull]
        public string? LoginPart { get; set; }
        [AllowNull]
        public string? AccurateEmail { get; set; }
        [AllowNull]
        public string? EmailPart { get; set; }
        [AllowNull]
        public string? Address { get; set; }
        [AllowNull]
        public string? Name { get; set; }
        [AllowNull]
        public IEnumerable<int>? RoleIds { get; set; }
    }
}
