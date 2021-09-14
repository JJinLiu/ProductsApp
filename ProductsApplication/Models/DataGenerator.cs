using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ProductsApplication.Models
{
    public static class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ProductContext(serviceProvider.GetRequiredService<DbContextOptions<ProductContext>>()))
            {
                if (context.Products.Any())
                {
                    return;   // Data was already seeded
                }

                context.Products.AddRange(
                    new Product
                    {
                        Id = 1,
                        Name = "ProductA"
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "ProductB"
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "ProductAAA"
                    });

                context.SaveChanges();
            }
        }
    }
}
