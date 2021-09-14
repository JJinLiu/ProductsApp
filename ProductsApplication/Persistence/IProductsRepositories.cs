using ProductsApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsApplication.Persistence
{
    public interface IProductsRepositories
    {
        public Product GetProductById(long productId);

        public Task AddProduct(Product product);

        public Task UpdateProduct(Product product);

        public IEnumerable<Product> ListProductsByName(string productName);
    }
}
