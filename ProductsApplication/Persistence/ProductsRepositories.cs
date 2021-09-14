using ProductsApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication.Persistence
{
    public class ProductsRepositories : IProductsRepositories
    {
        private readonly ProductContext _context;

        public ProductsRepositories(ProductContext context)
        {
            _context = context;
        }

        public Product GetProductById(long productId)
        {
            return _context.Products.Where(p => p.Id == productId && !p.Deleted).SingleOrDefault();
        }

        public async Task AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
             _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Product> ListProductsByName(string productName)
        {
            return _context.Products
                .Where(p => p.Name.ToLower().Contains(productName.ToLower()) && !p.Deleted)
                .OrderBy(p => p.Name);
        }
    }
}
