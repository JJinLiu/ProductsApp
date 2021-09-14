using FluentValidation;
using Microsoft.Extensions.Logging;
using ProductsApplication.Models;
using ProductsApplication.Persistence;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication.Services
{
    public class ProductServices : IProductServices
    {
        private readonly ILogger<ProductServices> _logger;
        private readonly IProductsRepositories _productsRepositories;

        public ProductServices(ILogger<ProductServices> logger, IProductsRepositories productsRepositories)
        {
            _productsRepositories = productsRepositories;
            _logger = logger;
        }

        public Product GetProduct(long productId)
        {
            var productResult = _productsRepositories.GetProductById(productId);
            if (productResult == null)
            {
                _logger.LogWarning($"Product was not found by Id: {productId}");
                throw new ObjectNotFoundException();
            }
            return productResult;
        }

        public async Task DeleleProductAsync(long productId)
        {
            var productResult = _productsRepositories.GetProductById(productId);
            if (productResult == null)
            {
                _logger.LogWarning($"Product was not found by Id: {productId}");
                throw new ObjectNotFoundException();
            }
            productResult.Deleted = true;
            await _productsRepositories.UpdateProduct(productResult);
        }

        public async Task AddProductAsync(Product product)
        {
            ValidateRequest(product);
            await _productsRepositories.AddProduct(product);
            _logger.LogInformation($"Record for product with Id: {product.Id} and Name: {product.Name} was created.");
        }

        public async Task UpdateProductAsync(long productId, Product product)
        {
            var productResult = _productsRepositories.GetProductById(productId);
            if (productResult == null)
            {
                _logger.LogWarning($"Product was not found by Id: {productId}");
                throw new ObjectNotFoundException();
            }

            ValidateRequest(product);
            productResult.Name = product.Name;
            await _productsRepositories.UpdateProduct(productResult);
            _logger.LogInformation($"Record for product with Id: {product.Id} was updated.");
        }

        public IEnumerable<Product> SearchProductsByName(string name)
        {
            var searchResult = _productsRepositories.ListProductsByName(name);
            if (searchResult.Any())
            {
                _logger.LogInformation($"Returned {searchResult.Count()} products from database.");
            }
            _logger.LogWarning($"No product was found by name: {name}.");
            return searchResult;
        }

        private void ValidateRequest(Product product)
        {
            var requestValidator = new ProductRequestValidator();
            var validationResult = requestValidator.Validate(product);
            if (!validationResult.IsValid && validationResult.Errors.Any())
            {
                var error = validationResult.Errors.FirstOrDefault().ErrorMessage;
                _logger.LogError($"Validation error with message: {error}");
                throw new ValidationException(error);
            }
        }
    }
}
