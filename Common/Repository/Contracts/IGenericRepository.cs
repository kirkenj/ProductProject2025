namespace Repository.Contracts
{
    public interface IGenericRepository<T, TIdType> where T : class, IIdObject<TIdType> where TIdType : struct
    {
        public Task<IReadOnlyCollection<T>> GetAllAsync();
        public Task<T?> GetAsync(TIdType id);
        public Task AddAsync(T obj);
        public Task DeleteAsync(TIdType id);
        public Task UpdateAsync(T obj);
        public Task<IReadOnlyCollection<T>> GetPageContent(int? page = default, int? pageSize = default);
    }
}
