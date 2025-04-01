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

        public async Task AddAsync(T obj)
        {
            await _collection.InsertOneAsync(obj);
        }

        public async Task DeleteAsync(TIdType id)
        {
            await _collection.DeleteOneAsync(filter => filter.Id!.Equals(id));
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            var selectResult = await _collection.FindAsync(_ => true);
            return await selectResult.ToListAsync();
        }

        public async Task<T?> GetAsync(TIdType id)
        {
            var selectResult = await _collection.FindAsync(filter => filter.Id!.Equals(id));
            return await selectResult.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetPageContent(int? page = null, int? pageSize = null)
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

            var selectionResult = await _collection.FindAsync(_ => true, findOptions);
            return await selectionResult.ToListAsync();
        }

        public async Task UpdateAsync(T obj)
        {
            await _collection.ReplaceOneAsync(filter => filter.Id!.Equals(obj.Id), obj);
        }
    }
}
