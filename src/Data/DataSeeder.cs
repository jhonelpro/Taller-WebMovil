using api.src.Models;
using api.src.Models.User;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.src.Data
{
    public class DataSeeder
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {

            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

                var faker = new Faker("es");
                var genders = new List<string> { "Femenino", "Masculino", "Prefiero no decirlo", "Otro" };
                Random random = new Random();

                if (!await userManager.Users.AnyAsync())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var email = faker.Internet.Email();

                        var user = new AppUser
                        {
                            UserName = email,
                            Email = email,
                            Name = faker.Name.FullName(),
                            Rut = faker.Random.Number(10000000, 99999999).ToString(),
                            DateOfBirth = faker.Date.Past(18),
                            Gender = genders[random.Next(0, genders.Count)],
                            IsActive = 1
                        };

                        var result = await userManager.CreateAsync(user, "Password1234!");

                        if (!result.Succeeded)
                        {
                            throw new Exception("Error creating user");
                        }
                    }

                    var admin = new AppUser
                    {
                        UserName = "admin@idwm.cl",
                        Email = "admin@idwm.cl",
                        Name = "Ignacio Mancilla",
                        Rut = "20.416.699-4",
                        Gender = "Masculino",
                        DateOfBirth = new DateTime(2000, 10, 25),
                        IsActive = 1
                    };

                    var adminResult = await userManager.CreateAsync(admin, "P4ssw0rd");

                    if (adminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        foreach (var error in adminResult.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
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