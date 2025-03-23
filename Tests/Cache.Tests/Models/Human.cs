namespace Cache.Tests.Models
{
    public class Human
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;


        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is Human human)
            {
                return this.Name == human.Name && this.Surname == human.Surname;
            }

            return base.Equals(obj);
        }
    }
}
