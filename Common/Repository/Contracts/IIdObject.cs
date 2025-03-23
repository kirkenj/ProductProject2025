namespace Repository.Contracts
{
    public interface IIdObject<T> where T : struct
    {
        T Id { get; }
    }
}
