using ProductsApplication.Controllers.Dto;
using ProductsApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsApplication.Services
{
    public interface IProductServices
    {
        public Product GetProduct(long productId);
        public Task DeleleProductAsync(long productId);
        public Task AddProductAsync(Product product);
        public Task UpdateProductAsync(long productId, Product product);
        public IEnumerable<Product> SearchProductsByName(string name);
    }
}
