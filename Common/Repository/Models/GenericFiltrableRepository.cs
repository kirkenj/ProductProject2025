using Microsoft.EntityFrameworkCore;
using Repository.Contracts;

namespace Repository.Models
{
    public class GenericFiltrableRepository<T, TIdType, TFilter> :
        GenericRepository<T, TIdType>,
        IGenericFiltrableRepository<T, TIdType, TFilter>
        where T : class, IIdObject<TIdType> where TIdType : struct
    {
        public GenericFiltrableRepository(DbContext dbContext, Func<IQueryable<T>, TFilter, IQueryable<T>> getFilteredSetDelegate)
            : base(dbContext)
        {
            GetFilteredSetDelegate = getFilteredSetDelegate;
        }

        private readonly Func<IQueryable<T>, TFilter, IQueryable<T>> GetFilteredSetDelegate;

        public virtual async Task<T?> GetAsync(TFilter filter) => await GetFilteredSetDelegate(DbSet.AsNoTracking(), filter).FirstOrDefaultAsync();

        public virtual async Task<IReadOnlyCollection<T>> GetPageContent(TFilter filter, int? page = default, int? pageSize = default)
        {
            var result = await GetPageContent(GetFilteredSetDelegate(DbSet.AsNoTracking(), filter), page, pageSize).ToArrayAsync();

            return result;
        }
    }
}
