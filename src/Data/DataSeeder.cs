using api.src.Models;
using api.src.Models.User;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.src.Data
{
    /// <summary>
    /// Clase DataSeeder para poblar la base de datos con datos iniciales.
    /// </summary>
    public class DataSeeder
    {
        /// <summary>
        /// Método estático para inicializar la base de datos con datos de prueba.
        /// </summary>
        /// <param name="serviceProvider">Parametro de tipo IServiceProvider que provee acceso a los servicios necesarios, como el contexto de la base de datos y el administrador de usuarios.</param>
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDBContext>();
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

                // Inicialización de Faker para datos en español
                var faker = new Faker("es");
                var genders = new List<string> { "Femenino", "Masculino", "Prefiero no decirlo", "Otro" };
                Random random = new Random();

                // Verifica si existen usuarios, y si no, crea usuarios de prueba
                if (!await userManager.Users.AnyAsync())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var email = faker.Internet.Email();

                        var rut = faker.Random.Number(10000000, 99999999).ToString() + "-" + faker.Random.Number(0, 9).ToString();
                        
                        var user = new AppUser
                        {
                            UserName = email,
                            Email = email,
                            Name = faker.Name.FullName(),
                            Rut = rut,
                            DateOfBirth = faker.Date.Past(18),
                            Gender = genders[random.Next(0, genders.Count)],
                            IsActive = 1
                        };
                    
                        var createUser = await userManager.CreateAsync(user, "Password1234!");

                        if (!createUser.Succeeded)
                        {
                            throw new Exception("Error al crear el usuario");
                        }

                        var roleResult = userManager.AddToRoleAsync(user, "User");

                        if (roleResult.Result.Succeeded)
                        {
                            Console.WriteLine($"Usuario {user.Email} creado exitosamente");
                        }
                        else
                        {
                            throw new Exception("Error al asignar rol al usuario");
                        }
                    }

                    // Creación de un usuario administrador con credenciales predeterminadas
                    var admin = new AppUser
                    {
                        UserName = "admin@idwm.cl",
                        Email = "admin@idwm.cl",
                        Name = "Ignacio Mancilla",
                        Rut = "20416699-4",
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

                // Verifica si existen tipos de productos en la base de datos y, si no, los agrega
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

                // Verifica si existen productos en la base de datos y, si no, los agrega usando Faker para generar datos aleatorios
                if (!context.Products.Any())
                {
                    var ProductFaker = new Faker<Product>()
                        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                        .RuleFor(p => p.Price, f => f.Random.Number(1000, 10000000))
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