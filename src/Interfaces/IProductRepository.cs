using api.src.DTOs;
using api.src.DTOs.Product;
using api.src.Helpers;
using api.src.Models;
using CloudinaryDotNet.Actions;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de productos en el repositorio, proporcionando métodos para CRUD y filtrado de productos.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">Parametro int que representa el ID del producto a buscar.</param>
        /// <returns>Producto correspondiente al ID, o null si no se encuentra.</returns>
        Task<Product?> GetProductById(int id);
        Task<List<ProductDtoForAdmin>> GetProducts(QueryObjectProduct query);
        Task<List<ProductDto>> GetAvailableProducts(QueryObject query);

        /// <summary>
        /// Obtiene una lista de productos para la vista del cliente según los criterios de consulta.
        /// </summary>
        /// <param name="query">Parametro de tipo QueryObjectProductClient que representa la consulta con los criterios del cliente.</param>
        /// <returns>Lista de productos que cumplen con los criterios de consulta del cliente.</returns>
        Task<List<ProductDto>> GetProductsClient(QueryObjectProductClient query);

        /// <summary>
        /// Agrega un nuevo producto a la base de datos.
        /// </summary>
        /// <param name="product">Parametro de tipo Product que representa el producto a agregar.</param>
        /// <param name="uploadResult">Parametro de tipo ImageUploadResult que representa el resultado de la carga de la imagen en Cloudinary.</param>
        /// <returns>Producto agregado al repositorio.</returns>
        Task<Product> AddProduct(Product product, ImageUploadResult uploadResult);

        /// <summary>
        /// Actualiza un producto existente basado en su ID y datos proporcionados.
        /// </summary>
        /// <param name="id">Parametro de tipo int que representa el ID del producto a actualizar.</param>
        /// <param name="updateProductRequestDto">Parametro de tipo UpdateProductRequestDto que representa los datos de la actualización del producto.</param>
        /// <param name="uploadResult">Parametro de tipo ImageUploadResult que representa el resultado de la carga de la imagen en Cloudinary.</param>
        /// <returns>Producto actualizado o null si no se encuentra el producto.</returns>
        Task<Product?> UpdateProduct(int id, UpdateProductRequestDto updateProductRequestDto, ImageUploadResult uploadResult);

        /// <summary>
        /// Elimina un producto del repositorio basado en su ID.
        /// </summary>
        /// <param name="id">Parametro de tipo int que representa el ID del producto a eliminar.</param>
        /// <returns>Producto eliminado o null si no se encuentra el producto.</returns>
        Task<Product?> DeleteProduct(int id);
    }
}