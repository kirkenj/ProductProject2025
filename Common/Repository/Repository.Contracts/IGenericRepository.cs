namespace Repository.Contracts
{
    public interface IGenericRepository<T, TIdType> where T : class, IIdObject<TIdType>
    {
        public Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<T?> GetAsync(TIdType id, CancellationToken cancellationToken = default);
        public Task AddAsync(T obj, CancellationToken cancellationToken = default);
        public Task DeleteAsync(TIdType id, CancellationToken cancellationToken = default);
        public Task UpdateAsync(T obj, CancellationToken cancellationToken = default);
        public Task<IReadOnlyCollection<T>> GetPageContent(int? page = default, int? pageSize = default, CancellationToken cancellationToken = default);
    }
}
