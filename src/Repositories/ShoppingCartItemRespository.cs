using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

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
                throw new Exception("Product id, quantity, and cart id cannot be less than or equal to zero.");
            }

            // Buscar el producto en la base de datos
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

            // Si el producto no existe, lanzar una excepción
            if (product == null)
            {
                throw new Exception("Product not found.");
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
                throw new Exception("Cart id cannot be less than or equal to zero.");
            }

            // Validar que la lista de items no sea nula
            if (cartItems == null)
            {
                throw new Exception("Cart items cannot be null.");
            }

            // Crear un nuevo objeto ShoppingCartItem con el ID del carrito
            var shoppingCartItem = new ShoppingCartItem { CartId = cartId };

            // Iterar sobre cada item en la lista proporcionada
            foreach (var item in cartItems)
            {
                // Buscar el producto en la base de datos
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                // Si el producto no existe, lanzar una excepción
                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                // Verificar si el item ya existe en el carrito
                var existingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                // Si el item no existe, se agrega como nuevo
                if (existingCartItem == null)
                {
                    shoppingCartItem.Product = product;
                    shoppingCartItem.ProductId = item.ProductId;
                    shoppingCartItem.Quantity = item.Quantity;

                    // Agregar el nuevo item al contexto y guardar los cambios
                    await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Si el item ya existe, incrementar su cantidad
                    existingCartItem.Quantity += item.Quantity;
                    await _context.SaveChangesAsync();
                }
            }

            // Retornar el último item agregado o actualizado
            return shoppingCartItem;
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
                throw new Exception("Product id, cart id, and quantity cannot be less than or equal to zero.");
            }

            // Buscar el producto en la base de datos
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

            // Si el producto no existe, lanzar una excepción
            if (product == null)
            {
                throw new Exception("Product not found.");
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
                throw new Exception("Product id cannot be less than or equal to zero.");
            }

            // Buscar el item en el carrito por el ID del producto
            var shoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => x.ProductId == productId);

            // Si no se encuentra el item, lanzar una excepción
            if (shoppingCartItem == null)
            {
                throw new Exception("Product not found.");
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
                throw new Exception("Cart id cannot be less than or equal to zero.");
            }

            // Obtener todos los items que pertenecen al carrito indicado
            var shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.CartId == cartId).ToListAsync();

            // Si no se encuentran items, lanzar una excepción
            if (shoppingCartItems == null || shoppingCartItems.Count == 0)
            {
                throw new Exception("Cart not found or no items in the cart.");
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
                throw new Exception("Product id cannot be less than or equal to zero.");
            }

            // Obtener el item del carrito a eliminar
            var existingCartItem = await GetShoppingCartItem(productId);

            // Si no se encuentra el item, lanzar una excepción
            if (existingCartItem == null)
            {
                throw new Exception("Product not found.");
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
                throw new Exception("Product id and quantity cannot be less than or equal to zero.");
            }

            // Obtener el item del carrito a actualizar
            var shoppingCartItem = await GetShoppingCartItem(productId);

            // Si no se encuentra el item, lanzar una excepción
            if (shoppingCartItem == null)
            {
                throw new Exception("Product not found.");
            }

            // Incrementar o decrementar la cantidad según el parámetro isIncrement
            if (isIncrement == true)
            {
                shoppingCartItem.Quantity += quantity; // Incrementar
            }
            else if (isIncrement == false)
            {
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