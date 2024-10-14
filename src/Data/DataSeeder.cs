using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;
using Bogus;

namespace api.src.Data
{
    public class DataSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();

                if (!context.ProductTypes.Any())
                {
                    context.ProductTypes.AddRange(
                        new ProductType { Name = "Poleras" },
                        new ProductType { Name = "Gorros" },
                        new ProductType { Name = "Juguetería" },
                        new ProductType { Name = "Alimentación" },
                        new ProductType { Name = "Libros" }
                    );
                }

                if (!context.Products.Any())
                {
                    var ProductFaker = new Faker<Product>()
                        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                        .RuleFor(p => p.Price, f => f.Random.Number(1000, 100000))
                        .RuleFor(p => p.Stock, f => f.Random.Number(0, 100000))
                        .RuleFor(p => p.ProductTypeId, f => f.Random.Number(1, 5)); // Asegúrate de que el rango es correcto.
                    
                    var products = ProductFaker.Generate(10);
                    context.Products.AddRange(products);
                }


                context.SaveChanges();
            }
        }
    }
}