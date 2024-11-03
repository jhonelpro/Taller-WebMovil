using api.src.Data;
using api.src.DTOs;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    /// <summary>
    /// Clase que implementa el repositorio de productos, proporcionando acceso a datos relacionados con productos.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        /// <summary>
        /// Atributo de tipo ApplicationDBContext que representa el contexto de la base de datos.
        /// </summary>
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Inicializa una nueva instancia de la clase ProductRepository con el contexto de base de datos especificado.
        /// </summary>
        /// <param name="context">El contexto de base de datos ApplicationDBContext que representa la conexión a la base de datos.</param>
        public ProductRepository(ApplicationDBContext context)
        {
            //Inicializa el atributo _context con el objeto de tipo ApplicationDBContext recibido.
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de productos según los criterios de búsqueda especificados en el objeto de consulta.
        /// </summary>
        /// <param name="query">Parámetro de tipo QueryObjectProduct que representa los criterios de búsqueda para los productos.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo una lista de objetos ProductDto que representan los productos encontrados.</returns>
        /// <exception cref="Exception">Lanzado si no se encuentran productos que coincidan con la búsqueda.</exception>
        public async Task<List<ProductDto>> GetProducts(QueryObjectProduct query)
        {
            // Obtiene los productos incluyendo su tipo.
            var products = _context.Products.Include(p => p.ProductType).AsQueryable();

            // Si el campo textFilter no esta vacio o no es nulo.
            if (!string.IsNullOrEmpty(query.textFilter))
            {
                // Se filtran los productos por el textFilter.
                products = products.Where(p => p.Name.Contains(query.textFilter) ||
                                               p.ProductType.Name.Contains(query.textFilter) ||
                                               p.Price.ToString().Contains(query.textFilter) ||
                                               p.Stock.ToString().Contains(query.textFilter));
                if (!products.Any())
                {
                    throw new Exception("Product not found.");
                }
            }

            // Paginacion
            var skipNumber = (query.pageNumber - 1) * query.pageSize;

            // Se retornan los productos.
            return await products.Skip(skipNumber).Take(query.pageSize)
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToListAsync();
        }

        /// <summary>
        /// Agrega un nuevo producto a la base de datos.
        /// </summary>
        /// <param name="product">Parámetro de tipo Product que representa el producto a agregar.</param>
        /// <param name="uploadResult">Parámetro de tipo ImageUploadResult que representa el resultado de la carga de la imagen del producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo el objeto Product que fue agregado.</returns>
        /// <exception cref="ArgumentNullException">Lanzado si el producto o el resultado de carga son nulos.</exception>
        /// <exception cref="Exception">Lanzado si el tipo de producto no se encuentra.</exception>
        public async Task<Product> AddProduct(Product product, ImageUploadResult uploadResult)
        {
            // Si el producto es nulo o el uploadResult es nulo.
            if (product == null || uploadResult == null) throw new ArgumentNullException("Product or UploadResult cannot be null.");
            
            // Se crea la instancia del producto, con los datos ingresados.
            var newProduct = new Product
            {
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = uploadResult.SecureUrl.AbsoluteUri
            };

            // Se asigna el tipo de producto al producto.
            newProduct.ProductType = await _context.ProductTypes.FirstOrDefaultAsync(p => p.Id == product.ProductTypeId) ?? throw new Exception("ProductType not found.");

            // Se aniade el producto a la base de datos.
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            //Se aniade la referencia entre el producto y tipo de producto en la base de datos.
            await _context.Entry(newProduct).Reference(p => p.ProductType).LoadAsync();

            // Retorna el producto aniadido.
            return newProduct;
        }

        /// <summary>
        /// Elimina un producto de la base de datos por su identificador.
        /// </summary>
        /// <param name="id">Parámetro de tipo int que representa el identificador del producto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo el objeto Product que fue eliminado.</returns>
        /// <exception cref="Exception">Lanzado si el producto no se encuentra.</exception>
        public async Task<Product?> DeleteProduct(int id)
        {
            // Se busca el producto en el contexto con el id ingresado.
            var product = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }
            
            // Se elimina el producto de la base de datos.
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Se retorna el producto eliminado.
            return product;
        }

        /// <summary>
        /// Obtiene una lista de productos disponibles según los criterios de búsqueda especificados en el objeto de consulta.
        /// </summary>
        /// <param name="query">Parámetro de tipo QueryObject que representa los criterios de búsqueda para los productos.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo una lista de objetos ProductDto que representan los productos encontrados.</returns>
        /// <exception cref="Exception">Lanzado si no se encuentran productos que coincidan con la búsqueda.</exception>
        public async Task<List<ProductDto>> GetAvailableProducts(QueryObject query)
        {
            // Se obtienen los productos incluyendo sus tipos.
            var products = _context.Products.Include(p => p.ProductType).AsQueryable();

            // Si el textFilter no esta vacio o no es nulo.
            if (!string.IsNullOrEmpty(query.textFilter))
            {
                // Se filtran los productos que contengan el textFilter en alguno de sus atributos.
                products = products.Where(p => p.Name.Contains(query.textFilter) ||
                                               p.ProductType.Name.Contains(query.textFilter) ||
                                               p.Price.ToString().Contains(query.textFilter) ||
                                               p.Stock.ToString().Contains(query.textFilter));
                
                if(!products.Any())
                {
                    throw new Exception("Product not found.");
                }
            }

            // Si el tipo de producto no esta vacio o no es nulo.
            if (!string.IsNullOrEmpty(query.productType))
            {
                // Se crea una lista con los tipos de productos validos.
                var validProductTypes = new[] { "Poleras", "Gorros", "Juguetería", "Alimentación", "Libros" };

                // Si el campo productType no contiene ninguno de los strings validos.
                if (!validProductTypes.Contains(query.productType))
                {
                    throw new Exception("Product Type incorrect.");
                }

                // Se filtran los productos, por el campo productType.
                products = products.Where(p => p.ProductType.Name.ToLower() == query.productType.ToLower());

                if(!products.Any())
                {
                    throw new Exception("Product not found.");
                }
            }

            // Si el campo sortByPrice no esta vacio o no es nulo.
            if(!string.IsNullOrWhiteSpace(query.sortByPrice))
            {
                // Si el campo sortByPrice es igual a Price.
                if(query.sortByPrice.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    // Se ordenan los productos por el precio de forma desdencte o ascendente.
                    products = query.IsDescending ? products.OrderByDescending(x => x.Price) : products.OrderBy(x => x.Price);
                }
            }

            // Paginacion.
            var skipNumber = (query.pageNumber - 1) * query.pageSize;

            // Se retornan los productos filtrados.
            return await products.Skip(skipNumber).Take(query.pageSize)
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una lista de productos para el cliente según los criterios de búsqueda especificados en el objeto de consulta.
        /// </summary>
        /// <param name="query">Parámetro de tipo QueryObjectProductClient que representa los criterios de búsqueda para los productos del cliente.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo una lista de objetos ProductDto que representan los productos encontrados.</returns>
        /// <exception cref="Exception">Lanzado si el tipo de producto es incorrecto o si no se encuentra el producto.</exception>
        public async Task<List<ProductDto>> GetProductsClient(QueryObjectProductClient query)
        {
            // Se obtienen los productos incluyendo su tipo.
            var products = _context.Products.Include(p => p.ProductType).AsQueryable();

            // Si el campo productType no esta vacio o no es nulo.
            if (!string.IsNullOrEmpty(query.productType))
            {
                // Se crean los strings validos para el nombre del productType.
                var validProductTypes = new[] { "Poleras", "Gorros", "Juguetería", "Alimentación", "Libros" };

                // Si el campo productType no contiene un string valido. 
                if (!validProductTypes.Contains(query.productType))
                {
                    throw new Exception("Product Type incorrect");
                }

                // Se filtran los productos por el campo productType.
                products = products.Where(p => p.ProductType.Name.ToLower() == query.productType.ToLower());

                if(!products.Any())
                {
                    throw new Exception("Product not found");
                }
            }

            // Si el campo sortByPrice no esta vacio o no es nulo.
            if(!string.IsNullOrWhiteSpace(query.sortByPrice))
            {
                // Si el campo sortByPrice es igual a Price.
                if(query.sortByPrice.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    // Se ordenan los productos por el precio de forma descendente o ascendente.
                    products = query.IsDescending ? products.OrderByDescending(x => x.Price) : products.OrderBy(x => x.Price);
                }
            }

            // Paginacion.
            var skipNumber = (query.pageNumber - 1) * query.pageSize;

            // Se retornan los producto filtrados, ordenados, paginados, y convertidos al formato de productDto.
            return await products.Skip(skipNumber).Take(query.pageSize)
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToListAsync();
        }

        /// <summary>
        /// Actualiza un producto existente en la base de datos.
        /// </summary>
        /// <param name="id">Parámetro de tipo int que representa el identificador del producto a actualizar.</param>
        /// <param name="product">Parámetro de tipo UpdateProductRequestDto que contiene los nuevos datos del producto.</param>
        /// <param name="uploadResult">Parámetro de tipo ImageUploadResult que representa el resultado de la carga de la nueva imagen del producto.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo el objeto Product que fue actualizado.</returns>
        /// <exception cref="Exception">Lanzado si el producto no se encuentra o el tipo de producto no se encuentra.</exception>
        public async Task<Product?> UpdateProduct(int id, UpdateProductRequestDto product, ImageUploadResult uploadResult)
        {
            // Se busca el producto a actualizar.
            var existingProduct = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingProduct == null)
            {
                throw new Exception("Product not found.");
            }

            // Se actualizan los atributos del producto.
            existingProduct.Name = product.Name;
            existingProduct.ProductTypeId = product.ProductTypeId;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            existingProduct.ProductType = await _context.ProductTypes.FirstOrDefaultAsync(p => p.Id == product.ProductTypeId) ?? throw new Exception("ProductType not found");
            
            // Se guardan los cambios en la base de datos.
            await _context.SaveChangesAsync();

            // Se retorna el producto actualizado.
            var productUpdated = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);
            return productUpdated;
        }

        /// <summary>
        /// Obtiene un producto por su identificador.
        /// </summary>
        /// <param name="id">Parámetro de tipo int que representa el identificador del producto a obtener.</param>
        /// <returns>Una tarea que representa la operación asincrónica, conteniendo el objeto Product que fue encontrado.</returns>
        /// <exception cref="Exception">Lanzado si el producto no se encuentra.</exception>
        public async Task<Product?> GetProductById(int id)
        {
            // Se busca el producto incluyendo su tipo, a travez del id.
            var product = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }
            
            // Se retorna el producto.
            return product;
        }
    }
}