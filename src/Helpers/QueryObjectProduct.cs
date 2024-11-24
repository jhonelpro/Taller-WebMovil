
namespace api.src.Helpers
{
    /// <summary>
    /// Clase QueryObjectProduct que representa la solicitud que permite el filtro y paginado de los productos.
    /// Contiene los parámetros necesarios para realizar filtros y paginar la lista de productos.
    /// </summary>
    public class QueryObjectProduct
    {
        /// <summary>
        /// Atributo de tipo string que representa el texto por el cual el usuario desea filtrar los productos.
        /// Permite buscar productos que coincidan con este texto.
        /// </summary>
        public string? textFilter { get; set; }= string.Empty;

        /// <summary>
        /// Atributo de tipo int que representa el número de página que se desea ver.
        /// </summary>
        public int pageNumber { get; set; } = 1;

        /// <summary>
        /// Atributo de tipo int que representa el tamaño de la página.
        /// Define el número de elementos que se verán en cada página de resultados.
        /// </summary>
        public int pageSize { get; set; } = 10;
    }
}