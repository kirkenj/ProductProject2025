namespace Repository.Contracts
{
    public interface IGenericFiltrableRepository<T, TIdType, TFilter> : IGenericRepository<T, TIdType> where T : class, IIdObject<TIdType> where TIdType : struct
    {
        public Task<T?> GetAsync(TFilter filter);
        public Task<IReadOnlyCollection<T>> GetPageContent(TFilter filter, int? page = default, int? pageSize = default);
    }
}
