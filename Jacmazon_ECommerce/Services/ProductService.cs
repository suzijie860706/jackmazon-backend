using Jacmazon_ECommerce.ViewModels;
using Jacmazon_ECommerce.Models.AdventureWorksLT2016Context;
using Jacmazon_ECommerce.Repositories;

namespace Jacmazon_ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly ICRUDRepository<Product> _repository;

        public ProductService(ICRUDRepository<Product> repository )
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProducts()
        {
            IEnumerable<Product> products = await _repository.FindAsync(x => true);
            IEnumerable<ProductViewModel> productResponseDtos = products.Select(x => new ProductViewModel { });
            return productResponseDtos;
        }
    }
}
