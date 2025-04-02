namespace Repository.Contracts
{
    public interface IGenericFiltrableRepository<T, TIdType, TFilter> : IGenericRepository<T, TIdType> where T : class, IIdObject<TIdType>
    {
        public Task<T?> GetAsync(TFilter filter, CancellationToken cancellationToken = default);
        public Task<IReadOnlyCollection<T>> GetPageContent(TFilter filter, int? page = default, int? pageSize = default, CancellationToken cancellationToken = default);
    }
}
