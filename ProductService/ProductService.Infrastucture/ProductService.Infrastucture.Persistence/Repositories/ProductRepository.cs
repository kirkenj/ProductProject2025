using Cache.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Core.Application.Models.Product;
using ProductService.Core.Domain.Models;
using Repository.Models.Relational;


namespace ProductService.Infrastucture.Persistence.Repositories
{
    public class ProductRepository : GenericFiltrableCachingRepository<Product, Guid, ProductFilter>, IProductRepository
    {
        public ProductRepository(ProductDbContext dbContext, ICustomMemoryCache customMemoryCache, ILogger<ProductRepository> logger) : base(dbContext, customMemoryCache, logger, GetFilteredSet)
        {
        }

        protected static IQueryable<Product> GetFilteredSet(IQueryable<Product> set, ProductFilter filter)
        {
            if (filter == null)
            {
                return set;
            }

            if (filter.Ids != null && filter.Ids.Any())
            {
                set = set.Where(obj => filter.Ids.Contains(obj.Id));
            }

            if (!string.IsNullOrEmpty(filter.NamePart))
            {
                set = set.Where(obj => obj.Name.Contains(filter.NamePart));
            }

            if (!string.IsNullOrEmpty(filter.DescriptionPart))
            {
                set = set.Where(obj => obj.Description.Contains(filter.DescriptionPart));
            }

            if (filter.PriceStart != null)
            {
                set = set.Where(obj => obj.Price >= filter.PriceStart);
            }

            if (filter.PriceEnd != null)
            {
                set = set.Where(obj => obj.Price <= filter.PriceEnd);
            }

            if (filter.IsAvailable != null)
            {
                set = set.Where(obj => obj.IsAvailable == filter.IsAvailable);
            }

            if (filter.ProducerIds != null && filter.ProducerIds.Any())
            {
                set = set.Where(obj => filter.ProducerIds.Contains(obj.ProducerId));
            }

            if (filter.CreationDateStart != null)
            {
                set = set.Where(obj => obj.CreationDate >= filter.CreationDateStart);
            }

            if (filter.CreationDateEnd != null)
            {
                set = set.Where(obj => obj.CreationDate <= filter.CreationDateEnd);
            }

            return set;
        }
    }
}
