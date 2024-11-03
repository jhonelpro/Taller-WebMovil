using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    /// <summary>
    /// Clase que implementa las operaciones de gestión de carritos de compras.
    /// Este repositorio interactúa con el contexto de la base de datos para realizar operaciones CRUD sobre los carritos de compras.
    /// </summary>
    public class ShoppingCartRepository : IShoppingCart
    {
        /// <summary>
        /// Contexto de la base de datos, utilizado para realizar consultas y operaciones sobre los datos.
        /// </summary>
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Constructor que inicializa el repositorio con un contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia del contexto de base de datos que se usará para realizar operaciones de acceso a datos.</param>
        public ShoppingCartRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea un nuevo carrito de compras asociado a un usuario específico.
        /// Se valida que el ID del usuario no sea nulo ni vacío antes de proceder a la creación.
        /// </summary>
        /// <param name="userId">ID del usuario para el cual se creará el carrito.</param>
        /// <returns>El objeto ShoppingCart creado y agregado a la base de datos.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el ID del usuario es nulo o vacío.</exception>
        public async Task<ShoppingCart> CreateShoppingCart(string userId)
        {
            // Validar que el ID del usuario no sea nulo ni vacío
            if (userId == null || string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("User id cannot be null or empty.");
            }

            if (await _context.ShoppingCarts.AnyAsync(x => x.UserId == userId))
            {
                throw new InvalidOperationException("Shopping cart already exists.");
            }
            
            var shoppingCart = new ShoppingCart
            {
                UserId = userId,
                Create_Date = DateTime.Now // Establecer la fecha de creación del carrito
            };

            // Agregar el nuevo carrito al contexto y guardar los cambios en la base de datos
            await _context.ShoppingCarts.AddAsync(shoppingCart);
            await _context.SaveChangesAsync();

            // Retornar el carrito creado
            return shoppingCart;
        }

        /// <summary>
        /// Obtiene el carrito de compras asociado a un usuario específico.
        /// Se valida que el ID del usuario no sea nulo ni vacío antes de realizar la consulta.
        /// </summary>
        /// <param name="userId">ID del usuario cuyo carrito se desea obtener.</param>
        /// <returns>El objeto ShoppingCart correspondiente al usuario, o null si no se encuentra.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el ID del usuario es nulo o vacío.</exception>
        public async Task<ShoppingCart?> GetShoppingCart(string userId)
        {
            // Validar que el ID del usuario no sea nulo ni vacío
            if (userId == null || string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("User id cannot be null or empty.");
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(x => x.shoppingCartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            

            // Retornar el carrito encontrado, o null si no existe
            return shoppingCart;
        }
    }
}