namespace Repository.Tests.Models
{
    public class UserFilter
    {
        public ICollection<Guid> Ids { get; set; } = null!;
        public string? LoginPart { get; set; } = null!;
        public string? EmailPart { get; set; } = null!;
        public string? NamePart { get; set; } = null!;
        public string? AddressPart { get; set; } = null!;


        public static IQueryable<User> GetFilteredSet(IQueryable<User> set, UserFilter filter)
        {
            if (filter == null)
            {
                return set;
            }

            if (filter.Ids != null && filter.Ids.Any())
            {
                set = set.Where(u => filter.Ids.Contains(u.Id));
            }

            if (!string.IsNullOrWhiteSpace(filter.LoginPart) && filter.LoginPart != string.Empty)
            {
                set = set.Where(u => u.Login.Contains(filter.LoginPart));
            }

            if (!string.IsNullOrWhiteSpace(filter.NamePart) && filter.NamePart != string.Empty)
            {
                set = set.Where(u => u.Name.Contains(filter.NamePart));
            }

            if (!string.IsNullOrWhiteSpace(filter.EmailPart) && filter.EmailPart != string.Empty)
            {
                set = set.Where(u => u.Email.Contains(filter.EmailPart));
            }

            if (!string.IsNullOrWhiteSpace(filter.AddressPart) && filter.AddressPart != string.Empty)
            {
                set = set.Where(u => u.Address.Contains(filter.AddressPart));
            }

            return set;
        }
    }
}
