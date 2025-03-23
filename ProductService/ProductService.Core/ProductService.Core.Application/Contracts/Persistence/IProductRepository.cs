using ProductService.Core.Application.Models.Product;
using ProductService.Core.Domain.Models;
using Repository.Contracts;


namespace ProductService.Core.Application.Contracts.Persistence
{
    public interface IProductRepository : IGenericFiltrableRepository<Product, Guid, ProductFilter>
    {
    }
}
