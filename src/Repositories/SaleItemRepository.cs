using api.src.Data;
using api.src.Models; 
using api.src.DTOs.Purchase;
using api.src.Interfaces;
using api.src.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api.src.Models.User;

namespace api.src.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de items de venta asociados a las compras en la aplicación.
    /// </summary>
    public class SaleItemRepository : ISaleItem
    {
        /// <summary>
        /// Atributo de tipo ApplicationDBContext que representa el contexto de la base de datos.
        /// </summary>
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Constructor de la clase SaleItemRepository.
        /// </summary>
        /// <param name="context">Parámetro de tipo ApplicationDBContext que representa el contexto de la base de datos.</param>
        public SaleItemRepository(ApplicationDBContext context)
        {
            // Asigna el contexto de la base de datos recibido al atributo _context.
            _context = context;
        }

        /// <summary>
        /// Crea los items de venta a partir de los items en el carrito de compras.
        /// </summary>
        /// <param name="shoppingCartItems">Parámetro de tipo List(ShoppingCartItem) que representa los items en el carrito de compras del usuario.</param>
        /// <param name="purchase">Parámetro de tipo Purchase que representa la compra realizada por el usuario.</param>
        /// <returns>Una lista de SaleItem que contiene los items de venta creados.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si los items del carrito o la compra son nulos.</exception>
        public async Task<List<SaleItem>> createSaleItem(List<ShoppingCartItem> shoppingCartItems, Purchase purchase)
        {
            // Verifica si la lista de items del carrito es nula.
            if (shoppingCartItems == null)
            {
                throw new ArgumentNullException("Shopping Cart Items cannot be null.");
            }

            // Verifica si la compra es nula.
            if (purchase == null)
            {
                throw new ArgumentNullException("Purchase cannot be null.");
            }

            // Lista para almacenar los items de venta que se crearán.
            var saleItems = new List<SaleItem>();

            // Itera sobre cada item del carrito para crear un item de venta correspondiente.
            foreach (var item in shoppingCartItems)
            {
                // Obtiene el producto asociado al ID del producto en el item del carrito.
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                // Verifica si el producto existe en la base de datos.
                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                // Crea un nuevo objeto SaleItem con los datos necesarios.
                var saleItem = new SaleItem
                {
                    PurchaseId = purchase.Id,          // Asocia el ID de la compra.
                    Purchase = purchase,               // Asigna el objeto de la compra.
                    Quantity = item.Quantity,          // Cantidad del producto comprado.
                    UnitPrice = product.Price,         // Precio unitario del producto.
                    TotalPrice = product.Price * item.Quantity,  // Calcula el precio total.
                    ProductId = item.ProductId,        // Asocia el ID del producto.
                    Product = product                  // Asigna el objeto del producto.
                };

                // Agrega el item de venta a la lista.
                saleItems.Add(saleItem);

                // Agrega el item de venta a la base de datos.
                await _context.SaleItems.AddAsync(saleItem);
            }

            // Guarda todos los cambios en la base de datos.
            await _context.SaveChangesAsync();
            return saleItems;
        }

        /// <summary>
        /// Obtiene una lista de compras realizadas por un usuario específico.
        /// </summary>
        /// <param name="userId">Parámetro de tipo string que representa el ID del usuario.</param>
        /// <returns>Una lista de objetos PurchaseDto que representa las compras realizadas por el usuario.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si el ID del usuario o las compras no existen.</exception>
        public async Task<List<PurchaseDto>> GetPurchasesAsync(string userId)
        {
            // Verifica si el ID del usuario es nulo o vacío.
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }

            // Recupera las compras realizadas por el usuario especificado.
            var purchases = await _context.Purchases
                                        .Where(x => x.UserId == userId)
                                        .ToListAsync();

            // Verifica si existen compras para el usuario.
            if (purchases == null || !purchases.Any())
            {
                throw new ArgumentNullException(nameof(purchases), "Purchases not found.");
            }

            // Recupera los items de venta relacionados con las compras obtenidas.
            var saleItems = await _context.SaleItems
                                        .Where(x => purchases.Select(y => y.Id).Contains(x.PurchaseId))
                                        .ToListAsync();

            // Verifica si existen items de venta para las compras.
            if (saleItems == null || !saleItems.Any())
            {
                throw new ArgumentNullException(nameof(saleItems), "Sale Items not found.");
            }

            // Lista para almacenar los DTOs de las compras.
            var purchasesDtos = new List<PurchaseDto>();

            // Itera sobre cada compra para construir el objeto PurchaseDto correspondiente.
            foreach (var purchase in purchases)
            {
                // Filtra los items de venta correspondientes a la compra actual.
                var relevantSaleItems = saleItems.Where(x => x.PurchaseId == purchase.Id).ToList();
                
                // Verifica si no hay items de venta para la compra.
                if (!relevantSaleItems.Any())
                {
                    continue; // O puedes lanzar una excepción según tu lógica
                }

                // Suma los precios totales de cada item de venta para obtener el total de la compra.
                double totalPrice = relevantSaleItems.Sum(item => item.TotalPrice);

                // Recupera los productos relacionados con los items de venta de la compra.
                var products = await _context.Products
                                            .Where(x => relevantSaleItems.Select(y => y.ProductId).Contains(x.Id))
                                            .Include(x => x.ProductType)
                                            .ToListAsync();

                // Verifica si los productos existen en la base de datos.
                if (products == null || !products.Any())
                {
                    throw new ArgumentNullException(nameof(products), "Products not found.");
                }

                // Crea el objeto PurchaseDto con la información relevante de la compra.
                var purchaseDto = new PurchaseDto
                {
                    PurchaseId = purchase.Id,
                    Email = purchase.User?.Email ?? "Unknown",
                    Country = purchase.Country,
                    City = purchase.City,
                    Commune = purchase.Commune,
                    Street = purchase.Street,
                    Purchase_TotalPrice = totalPrice,
                    saleItemDtos = PurchaseMapper.ToSaleItemDto(relevantSaleItems, products)
                };

                // Agrega el DTO de la compra a la lista de resultados.
                purchasesDtos.Add(purchaseDto);
            }

            return purchasesDtos;
        }

        /// <summary>
        /// Obtiene todas las compras realizadas, incluyendo la información del usuario, para administración.
        /// </summary>
        /// <returns>Una lista de objetos PurchaseDto que representa todas las compras en el sistema.</returns>
        public async Task<List<PurchaseDto>> GetPurchasesAsyncForAdmin()
        {
            // Obtiene todas las compras de la base de datos e incluye la información del usuario asociado a cada compra.
            var purchases = await _context.Purchases
                                        .Include(p => p.User) // Incluye la información del usuario relacionado con la compra.
                                        .ToListAsync();

            // Verifica si no se encontraron compras y lanza una excepción si es el caso.
            if (purchases == null || !purchases.Any())
            {
                throw new ArgumentNullException(nameof(purchases), "Purchases not found.");
            }

            // Extrae los IDs de las compras obtenidas para consultar los items de venta relacionados.
            var purchaseIds = purchases.Select(y => y.Id).ToList();
            
            // Obtiene los items de venta que están relacionados con las compras.
            var saleItems = await _context.SaleItems
                                        .Where(x => purchaseIds.Contains(x.PurchaseId)) // Filtra los items de venta por los IDs de compra.
                                        .ToListAsync();

            // Verifica si no se encontraron items de venta y lanza una excepción si es el caso.
            if (saleItems == null || !saleItems.Any())
            {
                throw new ArgumentNullException(nameof(saleItems), "Sale Items not found.");
            }

            // Extrae los IDs de los productos de los items de venta.
            var productIds = saleItems.Select(y => y.ProductId).ToList();
            
            // Obtiene los productos relacionados con los items de venta.
            var products = await _context.Products
                                        .Where(x => productIds.Contains(x.Id)) 
                                        .Include(x => x.ProductType) 
                                        .ToListAsync();

            // Verifica si no se encontraron productos y lanza una excepción si es el caso.
            if (products == null || !products.Any())
            {
                throw new ArgumentNullException(nameof(products), "Products not found.");
            }

            // Crea una lista para almacenar los DTOs de las compras.
            var purchasesDtos = new List<PurchaseDto>();

            // Itera sobre cada compra para construir el objeto PurchaseDto correspondiente.
            foreach (var purchase in purchases)
            {
                // Filtra los items de venta relevantes para la compra actual.
                var relevantSaleItems = saleItems.Where(x => x.PurchaseId == purchase.Id).ToList();
                
                // Calcula el precio total sumando los precios de los items de venta relevantes.
                var totalPrice = relevantSaleItems.Sum(item => item.TotalPrice);

                // Crea un objeto PurchaseDto con la información relevante de la compra.
                var purchaseDto = new PurchaseDto
                {
                    PurchaseId = purchase.Id,
                    Email = purchase.User?.Email ?? "Unknown", // Usa "Unknown" si el email del usuario es nulo.
                    Transaction_Date = purchase.Transaction_Date,
                    Country = purchase.Country,
                    City = purchase.City,
                    Commune = purchase.Commune,
                    Street = purchase.Street,
                    Purchase_TotalPrice = totalPrice,
                    saleItemDtos = PurchaseMapper.ToSaleItemDto(relevantSaleItems, products) // Mapea los items de venta a DTOs.
                };

                // Agrega el DTO de la compra a la lista de resultados.
                purchasesDtos.Add(purchaseDto);
            }

            return purchasesDtos;
        }

        /// <summary>
        /// Obtiene los items de venta asociados a una compra específica.
        /// </summary>
        /// <param name="purchaseId">Parámetro de tipo int que representa el ID de la compra.</param>
        /// <returns>Una lista de objetos SaleItem que representa los items de venta asociados a la compra.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si el ID de la compra o los items de venta no existen.</exception>
        public async Task<List<SaleItem>> getSaleItem(int purchaseId)
        {
            // Verifica si el ID de la compra es válido.
            if (purchaseId == 0)
            {
                throw new ArgumentNullException(nameof(purchaseId));
            }

            // Recupera los items de venta asociados al ID de la compra especificada.
            var saleItem = await _context.SaleItems
                                         .Where(x => x.PurchaseId == purchaseId)
                                         .ToListAsync();

            // Verifica si existen items de venta para la compra.
            if (saleItem == null)
            {
                throw new ArgumentNullException(nameof(saleItem));
            }

            return saleItem;
        }
    }
}