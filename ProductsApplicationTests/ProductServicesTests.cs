using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using ProductsApplication.Models;
using ProductsApplication.Persistence;
using ProductsApplication.Services;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ProductsApplicationTests
{
    public class ProductServicesTests
    {
        private readonly Mock<ILogger<ProductServices>> _mockLogger;
        private readonly Mock<IProductsRepositories> _mockRepository;
        private readonly ProductServices _productServices;

        public ProductServicesTests()
        {
            _mockLogger = new Mock<ILogger<ProductServices>>();
            _mockRepository = new Mock<IProductsRepositories>();
            _productServices = new ProductServices(_mockLogger.Object, _mockRepository.Object);
        }

        [Fact]
        public void Given_A_Product_Id_When_Call_Get_Product_By_Id_Then_The_Product_Will_Be_Returned()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "ProductA" };
            _mockRepository.Setup(x => x.GetProductById(It.IsAny<long>())).Returns(product);

            // Act
            var result = _productServices.GetProduct(product.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Equals("ProductA");       
        }

        [Fact]
        public void Given_A_Non_Existing_Product_Id_When_Call_Get_Product_By_Id_Then_There_Will_Be_Exception_Thrown()
        {
            // Act + Assert
            Assert.Throws<ObjectNotFoundException>(() => _productServices.GetProduct(1));
        }

        [Fact]
        public async Task Given_A_Product_Id_When_Call_Delete_Product_Then_The_Product_Will_Be_Marked_As_Deleted()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "ProductA" };
            _mockRepository.Setup(x => x.GetProductById(It.IsAny<long>())).Returns(product);

            // Act
            await _productServices.DeleleProductAsync(product.Id);

            // Assert
            product.Deleted.Should().BeTrue();
            _mockRepository.Verify(m => m.UpdateProduct(product));
        }

        [Fact]
        public async Task Given_A_Non_Existing_Product_Id_When_Call_Delete_Product_Then_There_Will_Be_Exception_Thrown()
        {
            // Act + Assert
            await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productServices.DeleleProductAsync(1));
        }

        [Fact]
        public async Task Given_A_Product_When_Call_Add_Product_Async_Then_Add_Product_Will_Be_Called()
        {
            // Arrange
            var product = new Product { Name = "ProductA" };

            // Act
            await _productServices.AddProductAsync(product);

            // Assert
            _mockRepository.Verify(m => m.AddProduct(product));
        }

        [Fact]
        public async Task Given_A_Product_With_Empty_Name_When_Call_Add_Product_Async_Then_Exception_Will_Be_Thrown()
        {
            // Arrange
            var product = new Product { Name = "" };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await _productServices.AddProductAsync(product));
        }

        [Fact]
        public async Task Given_An_Existing_Product_Id_When_Call_Upcdate_Product_Async_Then_The_Product_Will_Be_Updated()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "ProductA" };
            var productWithNewName = new Product { Name = "ProductB" };
            _mockRepository.Setup(x => x.GetProductById(It.IsAny<long>())).Returns(product);

            // Act
            await _productServices.UpdateProductAsync(1, productWithNewName);

            // Assert
            product.Name.Equals(productWithNewName.Name);
            _mockRepository.Verify(m => m.UpdateProduct(product));
        }

        [Fact]
        public async Task Given_A_Non_Existing_Product_Id_When_Call_Update_Product_Then_There_Will_Be_Exception_Thrown()
        {
            // Arrange
            var product = new Product { Name = "ProductA" };

            // Act + Assert
            await Assert.ThrowsAsync<ObjectNotFoundException>(async () => await _productServices.UpdateProductAsync(1, product));
        }


        [Fact]
        public async Task Given_A_Product_With_Empty_Name_When_Call_Update_Product_Async_Then_Exception_Will_Be_Thrown()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "ProductA" };
            var productWithNewName = new Product { Name = "" };
            _mockRepository.Setup(x => x.GetProductById(It.IsAny<long>())).Returns(product);

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await _productServices.UpdateProductAsync(1, productWithNewName));
        }

        [Fact]
        public void Given_A_Product_Name_When_Call_Search_Products_By_Name_Then_All_The_Products_Contain_The_Name_Will_Be_Returned()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "ProductA"
                },
                new Product
                {
                    Id = 2,
                    Name = "ProductB"
                }
            };

            _mockRepository.Setup(x => x.ListProductsByName("product")).Returns(products);

            // Act
            var result = _productServices.SearchProductsByName("product");

            // Assert
            result.Count().Should().Be(2);
            result.First().Id.Equals(1);
            result.First().Name.Equals("ProductA");
            result.ElementAt(1).Id.Equals(2);
            result.ElementAt(1).Name.Equals("ProductB");
        }
    }
}
