using Microsoft.EntityFrameworkCore;
using Repository.Contracts;

namespace Repository.Models
{
    public class GenericRepository<T, TIdType> : IGenericRepository<T, TIdType>
        where T : class, IIdObject<TIdType>
        where TIdType : struct
    {
        private readonly DbContext _dbContext = null!;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public virtual async Task AddAsync(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            await _dbContext.AddAsync(obj);

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        public virtual async Task DeleteAsync(TIdType id)
        {
            var val = await DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id));

            if (val == null) return;

            DbSet.Remove(val);

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }


        public virtual async Task<IReadOnlyCollection<T>> GetAllAsync() => await DbSet.AsNoTracking().ToArrayAsync();

        protected IQueryable<T> GetPageContent(IQueryable<T> query, int? page = default, int? pageSize = default)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var pageVal = page.Value <= 0 ? 1 : page.Value;
                var pageSizeVal = pageSize.Value <= 0 ? 1 : pageSize.Value;
                query = query.Skip((pageVal - 1) * pageSizeVal).Take(pageSizeVal);
            }

            return query;
        }

        public virtual async Task<IReadOnlyCollection<T>> GetPageContent(int? page = default, int? pageSize = default) =>
            await GetPageContent(DbSet.AsNoTracking(), page, pageSize).ToArrayAsync();


        public virtual async Task<T?> GetAsync(TIdType id) =>
            await DbSet.AsNoTracking().FirstOrDefaultAsync(o => o.Id.Equals(id));

        public virtual async Task UpdateAsync(T obj)
        {
            var currentDbVal = DbSet.First(o => o.Id.Equals(obj.Id));
            var ent = DbSet.Entry(currentDbVal);
            ent.CurrentValues.SetValues(obj);
            ent.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
