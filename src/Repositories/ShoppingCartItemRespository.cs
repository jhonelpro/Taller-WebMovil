using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.src.Repositories
{
    /// <summary>
    /// Clase que implementa las operaciones de gestión de items en el carrito de compras.
    /// Este repositorio interactúa con el contexto de la base de datos para realizar operaciones CRUD sobre los items del carrito.
    /// </summary>
    public class ShoppingCartItemRepository : IShoppingCartItem
    {
        /// <summary>
        /// Contexto de la base de datos, utilizado para realizar consultas y operaciones sobre los datos.
        /// </summary>
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Constructor que inicializa el repositorio con un contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia del contexto de base de datos, que se usará para realizar operaciones de acceso a datos.</param>
        public ShoppingCartItemRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega un nuevo item al carrito de compras.
        /// Se validan los IDs y la cantidad antes de intentar agregar el item.
        /// </summary>
        /// <param name="productId">ID del producto que se desea agregar al carrito.</param>
        /// <param name="cartId">ID del carrito donde se añadirá el item.</param>
        /// <param name="quantity">Cantidad del producto que se desea agregar.</param>
        /// <returns>El objeto ShoppingCartItem creado y agregado a la base de datos.</returns>
        /// <exception cref="Exception">Se lanza si los IDs del producto o carrito, o la cantidad, son menores o iguales a cero, o si el producto no se encuentra en la base de datos.</exception>
        public async Task<ShoppingCartItem> AddNewShoppingCartItem(int productId, int cartId, int quantity)
        {
            // Validar que los IDs y la cantidad son mayores a cero
            if (productId <= 0 || quantity <= 0 || cartId <= 0)
            {
                throw new Exception("El ID del producto, la cantidad y el ID del carrito no pueden ser menores o iguales a cero");
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(x => x.Id == productId);

            // Si el producto no existe, lanzar una excepción
            if (product == null)
            {
                throw new Exception("Producto no encontrado");
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.shoppingCartItems)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart == null)
            {
                throw new Exception("Carrito no encontrado");
            }

            var existingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .Include(s => s.shoppingCart)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                await _context.SaveChangesAsync();

                return existingCartItem;
            }
            
            var shoppingCartItem = new ShoppingCartItem
            {
                ProductId = productId,
                Product = product,
                CartId = cartId,
                Quantity = quantity
            };

            // Agregar el nuevo item al contexto y guardar los cambios
            await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
            await _context.SaveChangesAsync();

            // Retornar el item agregado
            return shoppingCartItem;
        }

        /// <summary>
        /// Agrega una lista de items al carrito de compras. Si un item ya existe, actualiza su cantidad.
        /// </summary>
        /// <param name="cartItems">Lista de ShoppingCartItem que se desea agregar al carrito.</param>
        /// <param name="cartId">ID del carrito donde se añadirán los items.</param>
        /// <returns>El último objeto ShoppingCartItem agregado o actualizado en el carrito.</returns>
        /// <exception cref="Exception">Se lanza si el ID del carrito es inválido, si la lista de items es nula, o si algún producto no se encuentra.</exception>
        public async Task<ShoppingCartItem> AddShoppingCarItem(List<ShoppingCartItem> cartItems, int cartId)
        {
            // Validar que el ID del carrito es mayor a cero
            if (cartId <= 0)
            {
                throw new ArgumentException("El ID del carrito no puede ser menor o igual a cero", nameof(cartId));
            }

            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Los artículos del carrito de compras no pueden ser nulos", nameof(cartItems));
            }

            // Obtiene el carrito y sus elementos en una sola consulta
            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.shoppingCartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart == null)
            {
                throw new KeyNotFoundException("El carro de compras no se encontró");
            }

            // Convierte los elementos existentes en un diccionario para búsqueda rápida
            var existingCartItems = shoppingCart.shoppingCartItems.ToDictionary(ci => ci.ProductId);

            // Crear un nuevo objeto ShoppingCartItem con el ID del carrito
            var shoppingCartItem = new ShoppingCartItem { CartId = cartId };

            // Iterar sobre cada item en la lista proporcionada
            foreach (var item in cartItems)
            {
                if (existingCartItems.TryGetValue(item.ProductId, out var existingCartItem))
                {        
                    existingCartItem.Quantity += item.Quantity;
                }
                else
                {
                    var product = await _context.Products
                        .Include(p => p.ProductType)
                        .FirstOrDefaultAsync(x => x.Id == item.ProductId);

                    if (product == null)
                    {
                        throw new KeyNotFoundException($"El producto con ID {item.ProductId} no fue encontrado");
                    }

                    var newCartItem = new ShoppingCartItem
                    {
                        CartId = cartId,
                        ProductId = item.ProductId,
                        Product = product,
                        Quantity = item.Quantity
                    };
                    shoppingCart.shoppingCartItems.Add(newCartItem);
                }
            }

            await _context.SaveChangesAsync();

            return shoppingCart.shoppingCartItems.FirstOrDefault() ?? new ShoppingCartItem();
        }

        public async Task<bool> ClearShoppingCart(int cartId)
        {
            if (cartId <= 0)
            {
                throw new ArgumentException("El ID del carrito no pueder ser igual o menor a cero", nameof(cartId));
            }

            var shoppingCartItems = await GetShoppingCartItems(cartId);
            
            if (shoppingCartItems == null || !shoppingCartItems.Any())
            {
                throw new InvalidOperationException("No se encontraron elementos en el carrito de compras");
            }

            _context.ShoppingCartItems.RemoveRange(shoppingCartItems);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Crea un nuevo item en el carrito de compras.
        /// </summary>
        /// <param name="productId">ID del producto a agregar.</param>
        /// <param name="cartId">ID del carrito donde se añadirá el item.</param>
        /// <param name="quantity">Cantidad del producto que se desea agregar.</param>
        /// <returns>El objeto ShoppingCartItem creado y agregado a la base de datos.</returns>
        /// <exception cref="Exception">Se lanza si los IDs del producto o carrito, o la cantidad, son menores o iguales a cero, o si el producto no se encuentra en la base de datos.</exception>
        public async Task<ShoppingCartItem> CreateShoppingCartItem(int productId, int cartId, int quantity)
        {
            // Validar que los IDs y la cantidad son mayores a cero
            if (productId <= 0 || cartId <= 0 || quantity <= 0)
            {
                throw new Exception("El ID del producto, la cantidad y el ID del carrito no pueden ser menores o iguales a cero");
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(x => x.Id == productId);

            // Si el producto no existe, lanzar una excepción
            if (product == null)
            {
                throw new Exception("Producto no encontrado");
            }

            // Crear un nuevo objeto ShoppingCartItem
            var shoppingCartItem = new ShoppingCartItem
            {
                ProductId = productId,
                Product = product,
                CartId = cartId,
                Quantity = quantity
            };

            // Agregar el nuevo item al contexto y guardar los cambios
            await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
            await _context.SaveChangesAsync();

            // Retornar el item creado
            return shoppingCartItem;
        }

        /// <summary>
        /// Obtiene un item del carrito de compras mediante el ID del producto.
        /// </summary>
        /// <param name="productId">ID del producto que se desea obtener.</param>
        /// <returns>El objeto ShoppingCartItem correspondiente al producto.</returns>
        /// <exception cref="Exception">Se lanza si el producto no se encuentra o si el ID es inválido.</exception>
        public async Task<ShoppingCartItem> GetShoppingCartItem(int productId)
        {
            // Validar que el ID del producto es mayor a cero
            if (productId <= 0)
            {
                throw new Exception("La id del producto no puede ser menor o igual a cero");
            }

            var shoppingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .Include(s => s.shoppingCart)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            // Si no se encuentra el item, lanzar una excepción
            if (shoppingCartItem == null)
            {
                throw new Exception("Producto no encontrado");
            }

            // Retornar el item encontrado
            return shoppingCartItem;
        }

        /// <summary>
        /// Obtiene todos los items en un carrito de compras específico.
        /// </summary>
        /// <param name="cartId">ID del carrito cuyo contenido se desea obtener.</param>
        /// <returns>Lista de objetos ShoppingCartItem que pertenecen al carrito especificado.</returns>
        /// <exception cref="Exception">Se lanza si el carrito no se encuentra o si el ID es inválido.</exception>
        public async Task<List<ShoppingCartItem>> GetShoppingCartItems(int cartId)
        {
            // Validar que el ID del carrito es mayor a cero
            if (cartId <= 0)
            {
                throw new Exception("La ID del carrito no puede ser menor o igual a cero");
            }

            var shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.CartId == cartId)
                .Include(s => s.shoppingCart)
                .Include(s => s.Product)
                .ToListAsync();

            // Si no se encuentran items, lanzar una excepción
            if (shoppingCartItems == null || shoppingCartItems.Count == 0)
            {
                throw new Exception("No se encontraron items en el carrito");
            }

            // Retornar la lista de items
            return shoppingCartItems;
        }

        /// <summary>
        /// Elimina un item del carrito de compras utilizando el ID del producto.
        /// </summary>
        /// <param name="productId">ID del producto que se desea eliminar del carrito.</param>
        /// <returns>El objeto ShoppingCartItem que fue eliminado.</returns>
        /// <exception cref="Exception">Se lanza si el ID del producto es inválido o si el producto no se encuentra en el carrito.</exception>
        public async Task<ShoppingCartItem> RemoveShoppingCartItem(int productId)
        {
            // Validar que el ID del producto es mayor a cero
            if (productId <= 0)
            {
                throw new Exception("La ID del producto no puede ser menor o igual a cero");
            }

            // Obtener el item del carrito a eliminar
            var existingCartItem = await GetShoppingCartItem(productId);

            // Si no se encuentra el item, lanzar una excepción
            if (existingCartItem == null)
            {
                throw new Exception("Producto no encontrado");
            }

            // Eliminar el item del contexto y guardar los cambios
            _context.ShoppingCartItems.Remove(existingCartItem);
            await _context.SaveChangesAsync();

            // Retornar el item eliminado
            return existingCartItem;
        }

        /// <summary>
        /// Actualiza la cantidad de un item en el carrito de compras.
        /// </summary>
        /// <param name="productId">ID del producto que se desea actualizar.</param>
        /// <param name="quantity">Nueva cantidad para el producto.</param>
        /// <param name="isIncrement">Indica si se debe incrementar (true) o decrementar (false) la cantidad.</param>
        /// <returns>El objeto ShoppingCartItem actualizado.</returns>
        /// <exception cref="Exception">Se lanza si el ID del producto es inválido, si la cantidad es menor o igual a cero, o si el producto no se encuentra en el carrito.</exception>
        public async Task<ShoppingCartItem> UpdateShoppingCartItem(int productId, int quantity, bool? isIncrement)
        {
            // Validar que el ID del producto y la cantidad son mayores a cero
            if (productId <= 0 || quantity <= 0)
            {
                throw new Exception("El ID del producto y la cantidad no pueden ser menores o iguales a cero");
            }

            // Obtener el item del carrito a actualizar
            var shoppingCartItem = await GetShoppingCartItem(productId);

            // Si no se encuentra el item, lanzar una excepción
            if (shoppingCartItem == null)
            {
                throw new Exception("Producto no encontrado");
            }

            // Incrementar o decrementar la cantidad según el parámetro isIncrement
            if (isIncrement == true)
            {
                if (shoppingCartItem.Quantity + quantity <= 0)
                {
                    throw new Exception("La cantidad debe ser mayor a cero");
                }

                shoppingCartItem.Quantity += quantity; // Incrementar
            }
            else if (isIncrement == false)
            {
                if (shoppingCartItem.Quantity - quantity <= 0)
                {
                    throw new Exception("La cantidad debe ser mayor a cero");
                }

                shoppingCartItem.Quantity -= quantity; // Decrementar
            }
            else
            {
                shoppingCartItem.Quantity = quantity; // Asignar nueva cantidad
            }

            // Actualizar el item en el contexto y guardar los cambios
            _context.ShoppingCartItems.Update(shoppingCartItem);
            await _context.SaveChangesAsync();

            // Retornar el item actualizado
            return shoppingCartItem;
        }
    }
}