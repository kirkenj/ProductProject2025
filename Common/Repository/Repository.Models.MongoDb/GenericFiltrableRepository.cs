using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Repository.Contracts;

namespace Repository.Models.MongoDb
{
    public class GenericFiltrableRepository<T, TIdType, TFilter> :
        GenericRepository<T, TIdType>,
        IGenericFiltrableRepository<T, TIdType, TFilter>
        where T : class, IIdObject<TIdType>
    {
        public GenericFiltrableRepository(IMongoDatabase mongoDatabase, Func<IQueryable<T>, TFilter, IQueryable<T>> getFilteredSetDelegate)
            : base(mongoDatabase)
        {
            GetFilteredSetDelegate = getFilteredSetDelegate;
        }

        private readonly Func<IQueryable<T>, TFilter, IQueryable<T>> GetFilteredSetDelegate;

        public virtual async Task<T?> GetAsync(TFilter filter, CancellationToken cancellationToken = default) => await GetFilteredSetDelegate(_collection.AsQueryable(), filter).FirstOrDefaultAsync(cancellationToken);

        public virtual async Task<IReadOnlyCollection<T>> GetPageContent(TFilter filter, int? page = default, int? pageSize = default, CancellationToken cancellationToken = default)
        {
            var set = GetFilteredSetDelegate(_collection.AsQueryable(), filter);

            if (page.HasValue && pageSize.HasValue)
            {
                var pageVal = page.Value <= 0 ? 1 : page.Value;
                var pageSizeVal = pageSize.Value <= 0 ? 1 : pageSize.Value;
                set = set.Skip((pageVal - 1) * pageSizeVal).Take(pageSizeVal);
            }

            return await set.ToListAsync(cancellationToken);
        }
    }
}
