using MongoDB.Driver;
using Repository.Contracts;

namespace Repository.Models.MongoDb
{
    public class GenericRepository<T, TIdType> : IGenericRepository<T, TIdType>
        where T : class, IIdObject<TIdType>
    {
        protected readonly IMongoCollection<T> _collection;

        public GenericRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<T>(typeof(T).FullName);
        }

        public async Task AddAsync(T obj, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(obj, new(), cancellationToken);
        }

        public async Task DeleteAsync(TIdType id, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteOneAsync(filter => filter.Id!.Equals(id), cancellationToken);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var selectResult = await _collection.FindAsync(_ => true, cancellationToken: cancellationToken);
            return await selectResult.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(TIdType id, CancellationToken cancellationToken = default)
        {
            var selectResult = await _collection.FindAsync(filter => filter.Id!.Equals(id), cancellationToken: cancellationToken);
            return await selectResult.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<T>> GetPageContent(int? page = null, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            FindOptions<T>? findOptions = null;

            if (page.HasValue && pageSize.HasValue)
            {
                findOptions = new FindOptions<T>();
                var pageVal = page.Value <= 0 ? 1 : page.Value;
                var pageSizeVal = pageSize.Value <= 0 ? 1 : pageSize.Value;
                findOptions.Skip = (pageVal - 1) * pageSizeVal;
                findOptions.Limit = pageSizeVal;
            }

            var selectionResult = await _collection.FindAsync(_ => true, findOptions, cancellationToken);
            return await selectionResult.ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
            await _collection.ReplaceOneAsync(filter => filter.Id!.Equals(obj.Id), obj, cancellationToken: cancellationToken);
        }
    }
}
