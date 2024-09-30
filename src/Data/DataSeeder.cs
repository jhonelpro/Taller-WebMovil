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

                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { Name = "Admin" },
                        new Role { Name = "User" }
                    );
                }

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

                var existingRuts = new HashSet<string>();

                if (!context.Users.Any(u => u.RoleId == 2))
                {
                    var UserFaker = new Faker<User>()
                        .RuleFor(u => u.Rut, f => GenerateUniqueRandomRut(existingRuts))
                        .RuleFor(u => u.Name, f => f.Person.FullName)
                        .RuleFor(u => u.Birth_Date, f => f.Person.DateOfBirth)
                        .RuleFor(u => u.Email, f => f.Person.Email)
                        .RuleFor(u => u.Password, f => f.Random.AlphaNumeric(8))
                        .RuleFor(u => u.RoleId, f => 2);
                    
                    var usesrs = UserFaker.Generate(10);
                    context.Users.AddRange(usesrs);
                }

                if (!context.Users.Any(u => u.RoleId == 1))
                {
                    var Admin = new Faker<User>()
                        .RuleFor(u => u.Rut, f => "20.416.699-4")
                        .RuleFor(u => u.Name, f => "Nombre: Ignacio Mancilla")
                        .RuleFor(u => u.Birth_Date, f => DateTime.Parse("25/10/2000"))
                        .RuleFor(u => u.Email, f => "admin@idwm.cl")
                        .RuleFor(u => u.Gender, f => "Masculino")
                        .RuleFor(u => u.Password, f => "P4ssw0rd")
                        .RuleFor(u => u.RoleId, f => 1);
                    
                    context.Users.AddRange(Admin);
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

        public static string GenerateUniqueRandomRut(HashSet<string> existingRuts)
        {
            string newRut;
            Random random = new Random();

            do
            {
                // Genera un RUT aleatorio (esto es solo un ejemplo simple)
                int numberPart = random.Next(10000000, 99999999); // Genera una parte numérica del RUT
                string verifierDigit = random.Next(0, 10).ToString(); // Genera el dígito verificador
                newRut = $"{numberPart}-{verifierDigit}";
            }
            while (existingRuts.Contains(newRut)); // Verifica si ya existe el RUT generado

            return newRut;
        }
    }
}