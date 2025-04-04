using System.Collections;

namespace Repository.Models.Relational.Test.Models
{
    public static class TestConstants
    {
        public static readonly List<User> DefaultUsers =
        [
            new()
            {
                Id = Guid.Parse("11f694b8-d482-4f88-a4bd-dee39633c50e"),
                Login = "Admin",
                Name = "Tom",
                Email = "Tom@gmail.com",
                Address = "Arizona"
            },
            new()
            {
                Id = Guid.Parse("ffb73e4f-0fd8-4921-9236-d4a437d65ff5"),
                Login = "kirk",
                Name = "Kan",
                Email = "Bad@mail.com",
                Address = "Grece"
            },
            new()
            {
                Id = Guid.Parse("c0577397-bffe-4de8-ab7c-7bfc576cdfef"),
                Login = "Samuel1999",
                Name = "Samuel",
                Email = "Samuel1999@bk.ru",
                Address = "Moskow"
            },
            new()
            {
                Id = Guid.Parse("329c87db-cc89-4cf3-9e48-ebf00ce7ba28"),
                Login = "Cat89",
                Name = "Alex",
                Email = "LionTheKing@ya.ru",
                Address = "Minsk"
            },
            new()
            {
                Id = Guid.Parse("09763208-bfd4-4d7b-9208-21e46ec7827e"),
                Login = "Monkey",
                Name = "Alan",
                Email = "Natan@gmail.com",
                Address = "Toronto"
            },
            new()
            {
                Id = Guid.Parse("8f81158f-85c8-4d17-9a7d-3a654053ee86"),
                Login = "Boss2012",
                Name = "Rick",
                Email = "Sanchezz@inbox.ru",
                Address = "London"
            }
        ];

        public class EmptyUserFilterData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return [new UserFilter()];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
